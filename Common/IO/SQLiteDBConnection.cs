using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace Vintagestory.API.Common
{
    public class SQLiteDBConnection : IDisposable
    {
        protected SQLiteConnection sqliteConn;
        protected string databaseFileName;
        protected ILogger logger;
        public object transactionLock = new object();

        public virtual string DBTypeCode => "database";
        public bool IsReadOnly { get; protected set; }

        public SQLiteDBConnection(ILogger logger)
        {
            this.logger = logger;
        }

        public bool OpenOrCreate(string filename, ref string errorMessage, bool requireWriteAccess, bool corruptionProtection, bool doIntegrityCheck)
        {
            databaseFileName = filename;

            if (!File.Exists(databaseFileName))
            {
                if (!HaveWriteAccessFolder(Path.GetDirectoryName(filename)))
                {
                    throw new IOException(string.Format("Cannot create {2} file {0}, the folder {1} does not seem to be writable!", filename, Path.GetDirectoryName(filename), DBTypeCode));
                }

            }
            else
            {
                if (!HaveWriteAccessFile(new FileInfo(filename)))
                {
                    if (requireWriteAccess)
                    {
                        throw new IOException(string.Format("Cannot open {1} file {0}, it seems to be not writable!", filename, DBTypeCode));
                    }

                    IsReadOnly = true;
                }
            }

            try
            {
                DbConnectionStringBuilder conf = new DbConnectionStringBuilder();
                conf.Add("Data Source", databaseFileName);
                conf.Add("Version", "3");
                conf.Add("New", "True"); // Create new file if it doesnt exist 
                conf.Add("Compress", "True");
                
                if (corruptionProtection)
                {
                    conf.Add("Journal Mode", "WAL");
                    conf.Add("Synchronous", "Normal");
                } else
                {
                    conf.Add("Journal Mode", "Off");
                }

                if (!requireWriteAccess)
                {
                    conf.Add("read only", "True");
                }

                sqliteConn = new SQLiteConnection(conf.ToString());
                
                sqliteConn.Open();

                if (requireWriteAccess)
                {
                    CreateTablesIfNotExists(sqliteConn);
                }
                

                if (doIntegrityCheck)
                {
                    if (!DoIntegrityCheck(sqliteConn))
                    {
                        logger.Error(errorMessage = "Database is possibly corrupted.");
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error(errorMessage = "Failed opening savegame.");
                logger.Error(e);
                return false;
            }

            OnOpened();

            return true;
        }

        public virtual void OnOpened()
        {

        }


        public virtual void Close()
        {
            sqliteConn?.Close();
            sqliteConn?.Dispose();
        }

        public virtual void Dispose()
        {
            Close();
        }


        protected virtual void CreateTablesIfNotExists(SQLiteConnection sqliteConn)
        {
            // Create your tables here
        }

        public void Vacuum()
        {
            using (SQLiteCommand command = sqliteConn.CreateCommand())
            {
                command.CommandText = "vacuum;";
                command.ExecuteNonQuery();
            }
        }


        public bool DoIntegrityCheck(SQLiteConnection sqliteConn, bool logResults = true)
        {
            bool okay = false;
            using (SQLiteCommand command = sqliteConn.CreateCommand())
            {
                command.CommandText = "PRAGMA integrity_check";

                using (SQLiteDataReader sqlite_datareader = command.ExecuteReader())
                {
                    if (logResults) logger.Notification(string.Format("Database: {0}. Running SQLite integrity check:", sqliteConn.DataSource));

                    while (sqlite_datareader.Read())
                    {
                        logger.Notification("Integrity check " + sqlite_datareader[0].ToString());
                        if (sqlite_datareader[0].ToString() == "ok")
                        {
                            okay = true;
                            break;
                        }
                    }
                }
            }

            return okay;
        }




        public static bool HaveWriteAccessFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException(folderPath);
            }

            try
            {
                string testfilename = Path.Combine(folderPath, "temp.txt");
                File.Create(testfilename).Close();
                File.Delete(testfilename);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }


        public static bool HaveWriteAccessFile(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            //file is not locked
            return true;
        }



        protected DbParameter CreateParameter(string parameterName, DbType dbType, object value, DbCommand command)
        {
            DbParameter p = command.CreateParameter();
            p.ParameterName = parameterName;
            p.DbType = dbType;
            p.Value = value;
            return p;
        }
    }

}

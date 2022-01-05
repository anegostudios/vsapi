using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

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
                StringBuilder b = new StringBuilder();
                DbConnectionStringBuilder.AppendKeyValuePair(b, "Data Source", databaseFileName);
                DbConnectionStringBuilder.AppendKeyValuePair(b, "Version", "3");
                DbConnectionStringBuilder.AppendKeyValuePair(b, "New", "True"); // Create new file if it doesnt exist 
                DbConnectionStringBuilder.AppendKeyValuePair(b, "Compress", "True");
                if (corruptionProtection)
                {
                    DbConnectionStringBuilder.AppendKeyValuePair(b, "Journal Mode", "WAL");
                    DbConnectionStringBuilder.AppendKeyValuePair(b, "Synchronous", "Normal");
                } else
                {
                    DbConnectionStringBuilder.AppendKeyValuePair(b, "Journal Mode", "Off");
                }
                
                sqliteConn = new SQLiteConnection(b.ToString());

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
                logger.Error(errorMessage = "Failed opening savegame." + e);
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
            sqliteConn.Close();
            sqliteConn.Dispose();
        }

        public virtual void Dispose()
        {
            Close();
        }


        protected virtual void CreateTablesIfNotExists(SQLiteConnection sqliteConn)
        {
            // Create your tables here
        }


        public void CompactDatabase(SQLiteConnection sqliteConn)
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

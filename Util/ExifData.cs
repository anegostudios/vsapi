// -------------------------------------------------------------------------------------------------
//
// Library "CompactExifLib" for reading and writing EXIF data in JPEG, TIFF and PNG image files.
//
// © Copyright 2021 Hans-Peter Kalb
//
// Version 1.6, Date 2021-06-25
// •Reading and writing of PNG images added.
//
// Version 1.5, Date 2021-05-30
// •Reading and writing of TIFF images added.
// •New property "ImageType" added.
// •Method "ImageFileBlockExists" addded. With this method it can be checked if an EXIF, XMP, IPTC or JPEG comment block exists.
// •Method "IsExifBlockEmpty" removed. Instead use the new method "ImageFileBlockExists".
// •Method "RemoveImageFileBlock" added. With this method an EXIF, XMP, IPTC or JPEG comment block can be removed.
// •All constants in the enum type "ExifSaveOptions" removed. Instead use the new method "RemoveImageFileBlock".
// •Static methods "ReadUInt16", WriteUInt16", "ReadUInt32" and "WriteUInt32" replaced by non-static methods
//  "ExifReadUInt16", "ExifWriteUInt16", "ExifReadUInt32" and "ExifWriteUInt32".
// •TIFF tag types SByte, SShort, Float and Double added, but these tag types are not fully supported.
// •Method "Empty" added.
// •First paramater of "ExifData" constructor must not be "null" any more. Instead use the new method "Empty".
//
// Version 1.4, Date 2021-03-31
// •Bug-fix: If a tag ID incorrectly exists more than once an exception is not thrown any more.
//  Instead the first occurrence of the tag ID is used.
// •New exception class "ExifException" added.
// •Load options for "ExifData" constructor added. Now an empty EXIF block can be created.
// •"ExifData" constructor without parameters removed.
// •When a JPEG file is saved the temporary file name contains the text "~" instead of "~temp".
//
// Version 1.3, Date 2021-03-15
// •"ExifData" constructor: Overloaded constructor added which can load from a stream.
// •"ExifData" constructor: Overloaded constructor added to create an empty EXIF block.
// •Method "Save": Overloaded method added which can save the EXIF data in a stream.
// •Method "GetTagValueCount": Parameter structure of the method changed. Info, if the tag exists, is now given back.
// •Method "SetTagValueCount": Overloaded
// method added which doesn't have a parameter for the tag type.
// •Method "GetTagType" added for reading the tag type.
//
// Version 1.2, Date 2021-02-13
// •Bug-fix: String comparison of file names changed from linguistic to ordinal comparison.
// •Method "IfdExists" added.
// •Type "ExifRational" extended by methods "ToDecimal" and "FromDecimal".
// •Type "GeoCoordinate" and special methods for reading and writing of GPS tags added: "Get-/SetGpsLongitude" etc.
// •Methods for reading and writing of date and time values with milliseconds added: "Get-/SetDateTaken" etc.
// •Searching for a tag ID speeded up by using the C# class "Dictionary" instead of "ArrayList".
// •Overloaded method "GetTagRawData" added which copies the raw data.
// •Type declarations "uint" and "ushort" removed from the enum types "ExifTag", "ExifIfd", "ExifTagId" and "ExifTagType".
// •Enum type "TimeFormat" renamed to "ExifDateFormat".
// •Method "GetByteCountOfTag" renamed to "GetTagByteCount".
// •Enum type "StrCodingFormat" added.
// •Demo application added.
//
// Version 1.1, Date 2020-05-16
// •Additional string coding constants for tag "UserComment" added.
// •Constant "StrCoding.Utf16Id_Undef" renamed to "StrCoding.IdCode_Utf16".
// •The method "Save" extended by an optional parameter. With this parameter, it is possible to remove JPEG blocks
//  with alternative description formats like IPTC-IIM, MPF and XMP block (Adobe Photoshop Information Resource Block).
// •In the method "Save" the order, in which the JPEG blocks are written, revised
//
// Version 1.0, Date 2020-05-01
// •Initial version.
//


// -------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable

namespace CompactExifLib
{
  public class ExifData
  {
    #region Public area

    public ImageType ImageType;
    public int MakerNoteOriginalOffset { get; private set; }
    public const int IfdShift = 16;


    // Load EXIF data from a JPEG or TIFF file.
    // An exception occurs in the following situations:
    //  •The file does not exist.
    //  •The access to the file is denied.
    //  •The file is not a valid JPEG or TIFF file.
    //
    // If the file is a valid JPEG file but without an EXIF block, an empty EXIF block is created.
    public ExifData(string FileNameWithPath, ExifLoadOptions Options = 0) : this()
    {
      _FileNameWithPath = Path.GetFullPath(FileNameWithPath);
      using (FileStream ImageFile = File.OpenRead(_FileNameWithPath))
      {
        ReadFromStream(ImageFile, Options);
      }
    }


    public ExifData(Stream ImageStream, ExifLoadOptions Options = 0) : this()
    {
      ReadFromStream(ImageStream, Options);
    }


    public static ExifData Empty()
    {
      ExifData EmptyBlock = new ExifData();
      EmptyBlock.SetEmptyExifBlock();
      return (EmptyBlock);
    }


    // Save the EXIF data in a new file or overwrite the current file.
    //
    // Parameters:
    //  "DestFileNameWithPath": File name for saving the image and the EXIF data. If this parameter is "null",
    //                          the EXIF data is replaced in the current file.
    public void Save(string DestFileNameWithPath = null, ExifSaveOptions SaveOptions = 0)
    {
      bool DestFileIsTempFile = false;
      string SourceFileNameWithPath = _FileNameWithPath;
      if (DestFileNameWithPath != null)
      {
        DestFileNameWithPath = Path.GetFullPath(DestFileNameWithPath);
      }
      if ((DestFileNameWithPath == null) ||
          (String.Compare(SourceFileNameWithPath, DestFileNameWithPath, StringComparison.OrdinalIgnoreCase) == 0))
      {
        int BackslashPosition = SourceFileNameWithPath.LastIndexOf('\\');
        int DotPosition = SourceFileNameWithPath.LastIndexOf('.');
        if (DotPosition <= BackslashPosition) DotPosition = SourceFileNameWithPath.Length;
        DestFileNameWithPath = SourceFileNameWithPath.Insert(DotPosition, "~");
        DestFileIsTempFile = true;
      }

      bool DestFileCreated = false;
      FileStream SourceFile = null, DestFile = null;
      try
      {
        SourceFile = File.OpenRead(SourceFileNameWithPath);
        DestFile = File.Open(DestFileNameWithPath, FileMode.Create);
        DestFileCreated = true;
        Save(SourceFile, DestFile, SaveOptions);
        SourceFile.Dispose();
        SourceFile = null;
        DestFile.Dispose();
        DestFile = null;
        if (DestFileIsTempFile)
        {
          File.Delete(SourceFileNameWithPath); // Delete original image file. If the original file cannot be deleted, delete
                                               // the temporary file within the "catch" block.
          File.Move(DestFileNameWithPath, SourceFileNameWithPath);
        }
      }
      catch
      {
        if (SourceFile != null)
        {
          SourceFile.Dispose();
        }
        if (DestFile != null)
        {
          DestFile.Dispose();
        }
        if (DestFileCreated)
        {
          File.Delete(DestFileNameWithPath);
        }
        throw;
      }
    }


    public void Save(Stream SourceStream, Stream DestStream, ExifSaveOptions SaveOptions = 0)
    {
      ImageType SourceImageType = CheckStreamTypeAndCompatibility(SourceStream);
      if (SourceImageType != ImageType)
      {
        throw new ExifException(ExifErrCode.ImageTypesDoNotMatch);
      }
      if (SourceImageType == ImageType.Jpeg)
      {
        SaveJpeg(SourceStream, DestStream);
      }
      else if (SourceImageType == ImageType.Tiff)
      {
        SaveTiff(SourceStream, DestStream);
      }
      else if (SourceImageType == ImageType.Png)
      {
        SavePng(SourceStream, DestStream);
      }
    }


    // Read a string from a tag.
    //
    // Parameters:
    //  "TagSpec":    Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "Value":      Return: Tag data as a string.
    //                In case of an error "null" is given back.
    //  "Coding":     Tag type and code page in which the string is coded.
    //  Return value: true  = The tag was successfully read.
    //                false = The tag was not found or is of wrong type.
    public bool GetTagValue(ExifTag TagSpec, out string Value, StrCoding Coding)
    {
            bool Success = false;
            int i, j;

      StrCodingFormat StringTagFormat = (StrCodingFormat)((uint)Coding & 0xFFFF0000);
      ushort CodePage = (ushort)Coding;
      if (StringTagFormat == StrCodingFormat.TypeUndefinedWithIdCode)
      {
        Success = GetTagValueWithIdCode(TagSpec, out Value, CodePage);
      }
      else
      {
        Value = null;
        if (GetTagItem(TagSpec, out TagItem t))
        {
          i = t.ValueCount;
          j = t.ValueIndex;

          if ((CodePage == 1200) || (CodePage == 1201)) // UTF16LE or UTF16BE
          {
            // Remove all null terminating characters. Here a null terminating character consists of 2 zero-bytes.
            while ((i >= 2) && (t.ValueData[j + i - 2] == 0) && (t.ValueData[j + i - 1] == 0))
            {
              i -= 2;
            }
          }
          else
          {
            // Remove all null terminating characters.
            while ((i >= 1) && (t.ValueData[j + i - 1] == 0))
            {
              i--;
            }
          }
          if (CodePage != 0)
          {
            if (((StringTagFormat == StrCodingFormat.TypeAscii) && (t.TagType == ExifTagType.Ascii)) ||
                ((StringTagFormat == StrCodingFormat.TypeUndefined) && (t.TagType == ExifTagType.Undefined)) ||
                ((StringTagFormat == StrCodingFormat.TypeByte) && (t.TagType == ExifTagType.Byte)))
            {
              Value = Encoding.GetEncoding(CodePage).GetString(t.ValueData, j, i);
              Success = true;
            }
          }
        }
      }
      return (Success);
    }


    // Write a string to a tag.
    //
    // Parameters:
    //  "TagSpec":    Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "Value":      String to be written.
    //  "Coding":     Tag type and code page in which the string should be coded.
    //  Return value: true  = The tag was successfully written.
    //                false = Error. Tag ID is not allowed to be written or IFD is invalid.
    public bool SetTagValue(ExifTag TagSpec, string Value, StrCoding Coding)
    {
      int TotalByteCount, StrByteLen, NullTermBytes;
      TagItem t;
      bool Success = false;
      byte[] StringAsByteArray;
      ExifTagType TagType;

      StrCodingFormat StringTagFormat = (StrCodingFormat)((uint)Coding & 0xFFFF0000);
      ushort CodePage = (ushort)Coding;
      if (StringTagFormat == StrCodingFormat.TypeUndefinedWithIdCode)
      {
        Success = SetTagValueWithIdCode(TagSpec, Value, CodePage);
      }
      else
      {
        if (StringTagFormat == StrCodingFormat.TypeUndefined)
        {
          TagType = ExifTagType.Undefined;
        }
        else if (StringTagFormat == StrCodingFormat.TypeByte)
        {
          TagType = ExifTagType.Byte;
        }
        else
        {
          TagType = ExifTagType.Ascii;
        }

        NullTermBytes = 0;
        if (TagType != ExifTagType.Undefined)
        {
          if ((CodePage == 1200) || (CodePage == 1201)) // UTF16LE or UTF16BE
          {
            NullTermBytes = 2; // Write null terminating character with 2 zero-bytes
          }
          else
          {
            NullTermBytes = 1; // Write null terminating character with 1 zero-byte
          }
        }

        if (CodePage != 0)
        {
          StringAsByteArray = Encoding.GetEncoding(CodePage).GetBytes(Value);
          StrByteLen = StringAsByteArray.Length;
          TotalByteCount = StrByteLen + NullTermBytes;
          t = PrepareTagForCompleteWriting(TagSpec, TagType, TotalByteCount);
          if (t != null)
          {
            Array.Copy(StringAsByteArray, 0, t.ValueData, t.ValueIndex, StrByteLen);
            if (NullTermBytes >= 1) t.ValueData[t.ValueIndex + StrByteLen] = 0;
            if (NullTermBytes >= 2) t.ValueData[t.ValueIndex + StrByteLen + 1] = 0;
            Success = true;
          }
        }
      }
      return (Success);
    }


    // Read a 32 bit signed integer number from a tag. The tag type must be one of the following values:
    //  ExifTagType.Byte, ExifTagType.UShort, ExifTagType.ULong, ExifTagType.SLong.
    //
    // Parameters:
    //  "TagSpec":    Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "Value":      Return: 32 bit integer number with sign. If the tag type is "ExifTagType.ULong" and
    //                the tag contains a number from 0x80000000 to 0xFFFFFFFF, it is treated as an error.
    //                In case of an error, "Value" is 0.
    //  "Index":      Array index of the value to be read.
    //  Return value: true  = The tag was successfully read.
    //                false = The tag was not found, is of wrong type or is out of range.
    public bool GetTagValue(ExifTag TagSpec, out int Value, int Index = 0)
    {
            bool Success = false;
            uint TempValue = 0;

      if (GetTagItem(TagSpec, out TagItem t) && ReadUintElement(t, Index, out TempValue))
      {
        Success = ((t.TagType != ExifTagType.ULong) || ((int)TempValue >= 0));
        if (!Success) TempValue = 0;
      }
      Value = (int)TempValue;
      return (Success);
    }


    // Write a 32 bit signed integer number to a tag.
    //
    // Parameters:
    //  "TagSpec":    Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "TagType":    Tag type: ExifTagType.Byte, ExifTagType.UShort, ExifTagType.ULong or
    //                ExifTagType.SLong
    //  "Value":      32 bit integer number with sign.
    //  "Index":      Array index of the value to be written.
    //  Return value: true  = The tag was successfully written.
    //                false = The tag specification is invalid or "Value" is out of range.
    public bool SetTagValue(ExifTag TagSpec, int Value, ExifTagType TagType, int Index = 0)
    {
      TagItem t;
      bool Success = false;

      switch (TagType)
      {
        case ExifTagType.Byte:
          if ((Value >= 0) && (Value <= 255)) Success = true;
          break;
        case ExifTagType.UShort:
          if ((Value >= 0) && (Value <= 65535)) Success = true;
          break;
        case ExifTagType.ULong:
          if (Value >= 0) Success = true;
          break;
        case ExifTagType.SLong:
          Success = true;
          break;
      }
      if (Success)
      {
        t = PrepareTagForArrayItemWriting(TagSpec, TagType, Index);
        Success = WriteUintElement(t, Index, (uint)Value);
      }
      return (Success);
    }


    public bool GetTagValue(ExifTag TagSpec, out uint Value, int Index = 0)
    {
            bool Success = false;
            uint TempValue = 0;

      if (GetTagItem(TagSpec, out TagItem t) && ReadUintElement(t, Index, out TempValue))
      {
        Success = ((t.TagType != ExifTagType.SLong) || ((int)TempValue >= 0));
        if (!Success) TempValue = 0;
      }
      Value = TempValue;
      return (Success);
    }


    public bool SetTagValue(ExifTag TagSpec, uint Value, ExifTagType TagType, int Index = 0)
    {
      TagItem t;
      bool Success = false;

      switch (TagType)
      {
        case ExifTagType.Byte:
          if ((Value >= 0) && (Value <= 255)) Success = true;
          break;
        case ExifTagType.UShort:
          if ((Value >= 0) && (Value <= 65535)) Success = true;
          break;
        case ExifTagType.ULong:
          Success = true;
          break;
        case ExifTagType.SLong:
          if ((int)Value >= 0) Success = true;
          break;
      }
      if (Success)
      {
        t = PrepareTagForArrayItemWriting(TagSpec, TagType, Index);
        Success = WriteUintElement(t, Index, (uint)Value);
      }
      return (Success);
    }


    public bool GetTagValue(ExifTag TagSpec, out ExifRational Value, int Index = 0)
    {
            bool Success = false;

            if (GetTagItem(TagSpec, out TagItem t) && ReadURatElement(t, Index, out uint Numer, out uint Denom) == true)
            {
                if (t.TagType == ExifTagType.URational)
                {
                    Value = new ExifRational(Numer, Denom);
                }
                else // ExifTagType.SRational
                {
                    Value = new ExifRational((int)Numer, (int)Denom);
                }
                Success = true;
            }
            else
            {
                Value = new ExifRational(0, 0);
            }
            return (Success);
    }


    public bool SetTagValue(ExifTag TagSpec, ExifRational Value, ExifTagType TagType, int Index = 0)
    {
      TagItem t;
      bool Success = false;

      switch (TagType)
      {
        case ExifTagType.SRational:
          if ((Value.Numer < 0x80000000) && (Value.Denom < 0x80000000))
          {
            if (Value.Sign)
            {
              Value.Numer = (uint)(-(int)Value.Numer);
            }
            Success = true;
          }
          else Success = false;
          break;
        case ExifTagType.URational:
          Success = !Value.IsNegative();
          break;
      }
      if (Success)
      {
        t = PrepareTagForArrayItemWriting(TagSpec, TagType, Index);
        Success = WriteURatElement(t, Index, Value.Numer, Value.Denom);
      }
      return (Success);
    }


    // Read time stamp from a tag. The tag type must be "ExifTagType.Ascii" and the tag
    // must contain a null terminated string with 19 characters in the 
    // format "yyyy:MM:dd HH:mm:ss" or with 10 characters in the format "yyyy:MM:dd".
    //
    // Parameters:
    //  "TagSpec":    Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "Value":      Return: Date time stamp. In case of an error, the value "DateTime.MinValue" is returned.
    //  "Format"      Selection between time stamp format with 19 or 10 characters.
    //  Return value: true  = The tag was successfully read.
    //                false = The tag was not found, is of wrong type or wrong format.
    public bool GetTagValue(ExifTag TagSpec, out DateTime Value, ExifDateFormat Format = ExifDateFormat.DateAndTime)
    {
            bool Success;
            int i, Year, Month, Day, Hour, Minute, Second;

      Success = false;
      Value = DateTime.MinValue;
      if (GetTagItem(TagSpec, out TagItem t) && (t.TagType == ExifTagType.Ascii))
      {
        try
        {
          i = t.ValueIndex;
          if ((t.ValueCount >= 10) && (t.ValueData[i + 4] == (byte)':') && (t.ValueData[i + 7] == (byte)':'))
          {
            Year = CalculateTwoDigitDecNumber(t.ValueData, i) * 100 + CalculateTwoDigitDecNumber(t.ValueData, i + 2);
            Month = CalculateTwoDigitDecNumber(t.ValueData, i + 5);
            Day = CalculateTwoDigitDecNumber(t.ValueData, i + 8);
            if ((Format == ExifDateFormat.DateAndTime) && (t.ValueCount == 19 + 1) && (t.ValueData[i + 10] == (byte)' ') &&
                (t.ValueData[i + 13] == (byte)':') && (t.ValueData[i + 16] == (byte)':') && (t.ValueData[i + 19] == 0))
            {
              Hour = CalculateTwoDigitDecNumber(t.ValueData, i + 11);
              Minute = CalculateTwoDigitDecNumber(t.ValueData, i + 14);
              Second = CalculateTwoDigitDecNumber(t.ValueData, i + 17);
              Value = new DateTime(Year, Month, Day, Hour, Minute, Second);
              Success = true;
            }
            else if ((Format == ExifDateFormat.DateOnly) && (t.ValueCount == 10 + 1) && (t.ValueData[i + 10] == 0))
            {
              Value = new DateTime(Year, Month, Day);
              Success = true;
            }
          }
        }
        catch
        {
          // Invalid time stamp
        }
      }
      return (Success);
    }


    // Write a time stamp to a tag. The tag type is set to "ExifTagType.Ascii" and the time stammp
    // is written in the format "yyyy:MM:dd HH:mm:ss" or "yyyy:MM:dd".
    //
    // Parameters:
    //  "TagSpec":    Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "Value":      Date time stamp.
    //  "Format"      Selection between time stamp format with 19 or 10 characters.
    //  Return value: true  = The tag was successfully written.
    //                false = Error.
    public bool SetTagValue(ExifTag TagSpec, DateTime Value, ExifDateFormat Format = ExifDateFormat.DateAndTime)
    {
      TagItem t;
      bool Success = false;
      int i, sByteCount = 0, Year;

      if (Format == ExifDateFormat.DateAndTime)
      {
        sByteCount = 19 + 1; // "+ 1" because a null character has to be written
      }
      else if (Format == ExifDateFormat.DateOnly)
      {
        sByteCount = 10 + 1;
      }
      if (sByteCount != 0)
      {
        t = PrepareTagForCompleteWriting(TagSpec, ExifTagType.Ascii, sByteCount);
        if (t != null)
        {
          i = t.ValueIndex;
          Year = Value.Year;
          ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Year / 100);
          ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Year % 100);
          t.ValueData[i] = (byte)':'; i++;
          ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Value.Month);
          t.ValueData[i] = (byte)':'; i++;
          ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Value.Day);
          if (Format == ExifDateFormat.DateAndTime)
          {
            t.ValueData[i] = (byte)' '; i++;
            ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Value.Hour);
            t.ValueData[i] = (byte)':'; i++;
            ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Value.Minute);
            t.ValueData[i] = (byte)':'; i++;
            ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Value.Second);
          }
          t.ValueData[i] = 0; i++;
          Success = true;
        }
      }
      return (Success);
    }


    // Read the tag data as raw data without copying the raw data.
    //
    // Parameters:
    //  "TagSpec":          Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "TagType":          Return: Type of the tag.
    //  "ValueCount":       Return: Number of values which are stored in the tag.
    //  "RawData" :         Return: Array with the tag data, but there may also be other data stored in this array.
    //                      There are the following restrictions:
    //                      *The data returned in the array "RawData" must not be changed by the caller of this method.
    //                       "RawData" should only be used for reading data!
    //                      *After new data have been written into the specified tag, the array "RawData" must not be used any
    //                       more. Therefore the data returned in "RawData" should be copied as soon as possible.
    //                      *If the tag does not exist, "RawData" is null.
    //  "RawDataIndex" :    Return: Array index at which the data starts. The first data byte is stored in 
    //                      "RawData[RawDataIndex]" and the last data byte is stored in 
    //                      "RawData[RawDataIndex + GetTagByteCount(TagType, ValueCount) - 1]".
    //  Return value:       true  = The tag was successfully read.
    //                      false = The tag was not found.
    public bool GetTagRawData(ExifTag TagSpec, out ExifTagType TagType, out int ValueCount, out byte[] RawData,
      out int RawDataIndex)
    {
      bool Success = false;

      if (GetTagItem(TagSpec, out TagItem t))
      {
        TagType = t.TagType;
        ValueCount = t.ValueCount;
        RawData = t.ValueData;
        RawDataIndex = t.ValueIndex;
        Success = true;
      }
      else
      {
        TagType = 0;
        ValueCount = 0;
        RawData = null;
        RawDataIndex = 0;
      }
      return (Success);
    }


    // Read the tag data as raw data with copying the raw data.
    //
    // Parameters:
    //  "TagSpec":          Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "TagType":          Return: Type of the tag.
    //  "ValueCount":       Return: Number of values which are stored in the tag.
    //  "RawData" :         Return: Array with the tag data. If the tag does not exist, "RawData" is null.
    //  Return value:       true  = The tag was successfully read.
    //                      false = The tag was not found.
    public bool GetTagRawData(ExifTag TagSpec, out ExifTagType TagType, out int ValueCount, out byte[] RawData)
    {
      bool Success = false;

      if (GetTagItem(TagSpec, out TagItem t))
      {
        TagType = t.TagType;
        ValueCount = t.ValueCount;
        int k = t.ByteCount;
        RawData = new byte[k];
        Array.Copy(t.ValueData, t.ValueIndex, RawData, 0, k);
        Success = true;
      }
      else
      {
        TagType = 0;
        ValueCount = 0;
        RawData = null;
      }
      return (Success);
    }


    // Write the tag data as raw data.
    //
    // Parameters:
    //  "TagSpec":      Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  "TagType":      Type of the tag.
    //  "ValueCount":   Number of values which are stored in the tag. This is not the number of raw data bytes!
    //                  The number of raw data bytes is automatically calculated by the parameters "TagType" and
    //                  "ValueCount".
    //  "RawData":      Array with the tag data. The tag data is not copied by this method, therefore
    //                  the array content must not be changed after the call of this method.
    //  "RawDataIndex": Array index at which the data starts. Set this parameter to 0, if the data starts with
    //                  the first element of the array "RawData".
    //  Return value:   true  = The tag was successfully set.
    //                  false = The IFD is invalid.
    public bool SetTagRawData(ExifTag TagSpec, ExifTagType TagType, int ValueCount, byte[] RawData, int RawDataIndex = 0)
    {
            bool Success = false;

            ExifIfd Ifd = ExtractIfd(TagSpec);
      if ((uint)Ifd < ExifIfdCount)
      {
        int RawDataByteCount = GetTagByteCount(TagType, ValueCount);
        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
        if (IfdTagTable.TryGetValue(ExtractTagId(TagSpec), out TagItem t))
        {
          t.TagType = TagType;
          t.ValueCount = ValueCount;
          t.ValueData = RawData;
          t.ValueIndex = RawDataIndex;
          t.AllocatedByteCount = RawDataByteCount;
        }
        else
        {
          t = new TagItem(ExtractTagId(TagSpec), TagType, ValueCount, RawData, RawDataIndex, RawDataByteCount);
          IfdTagTable.Add(t.TagId, t);
        }
        Success = true;
      }
      return (Success);
    }


    public bool GetTagValueCount(ExifTag TagSpec, out int ValueCount)
    {
      if (GetTagItem(TagSpec, out TagItem t))
      {
        ValueCount = t.ValueCount;
        return (true);
      }
      else
      {
        ValueCount = 0;
        return (false);
      }
    }


    public bool SetTagValueCount(ExifTag TagSpec, int ValueCount)
    {
      if (GetTagItem(TagSpec, out TagItem t) && ((uint)ValueCount <= MaxTagValueCount))
      {
        t.SetTagTypeAndValueCount(t.TagType, ValueCount, true);
        return (true);
      }
      else return (false);
    }


    public bool SetTagValueCount(ExifTag TagSpec, int ValueCount, ExifTagType TagType)
    {
      TagItem t = PrepareTagForArrayItemWriting(TagSpec, TagType, ValueCount - 1);
      return (t != null);
    }


    public bool GetTagType(ExifTag TagSpec, out ExifTagType TagType)
    {
      if (GetTagItem(TagSpec, out TagItem t))
      {
        TagType = t.TagType;
        return (true);
      }
      else
      {
        TagType = 0;
        return (false);
      }
    }


    public bool TagExists(ExifTag TagSpec)
    {
      ExifIfd Ifd = ExtractIfd(TagSpec);
      if ((uint)Ifd < ExifIfdCount)
      {
        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
        return (IfdTagTable.ContainsKey(ExtractTagId(TagSpec)));
      }
      else return (false);
    }


    public bool IfdExists(ExifIfd Ifd)
    {
      if ((uint)Ifd < ExifIfdCount)
      {
        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
        return (IfdTagTable.Count > 0);
      }
      else return (false);
    }


    public bool ImageFileBlockExists(ImageFileBlock BlockType)
    {
      return (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Existent);
    }


    // Remove a single tag.
    //
    // Parameters:
    //  "TagSpec":      Tag specification, consisting of the IFD in which the tag is stored and the tag ID.
    //  Return value:   false = Tag was not removed, because it doesn't exist.
    //                  true  = Tag was removed.
    public bool RemoveTag(ExifTag TagSpec)
    {
      ExifIfd Ifd = ExtractIfd(TagSpec);
      if ((uint)Ifd < ExifIfdCount)
      {
        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
        return (IfdTagTable.Remove(ExtractTagId(TagSpec)));
      }
      else return (false);
    }


    // Remove all tags from a specific IFD.
    //
    // Parameters:
    //  "ExifIfd": IFD from which all tags are to be removed. If the IFD "ThumbnailData" is removed, the thumbnail image
    //             is removed, too.
    public bool RemoveAllTagsFromIfd(ExifIfd Ifd)
    {
      bool Removed = false;

      if (Ifd == ExifIfd.PrimaryData)
      {
        RemoveAllTagsFromIfdPrimaryData();
        Removed = true;
      }
      else if ((uint)Ifd < ExifIfdCount)
      {
        ClearIfd_Unchecked(Ifd);
        if (Ifd == ExifIfd.ThumbnailData)
        {
          RemoveThumbnailImage(false);
        }
        Removed = true;
      }
      return (Removed);
    }


    // Remove all tags from the EXIF block and remove the thumbnail image.
    public void RemoveAllTags()
    {
      RemoveAllTagsFromIfdPrimaryData();
      for (ExifIfd Ifd = ExifIfd.PrivateData; Ifd < (ExifIfd)ExifIfdCount; Ifd++)
      {
        ClearIfd_Unchecked(Ifd);
      }
      RemoveThumbnailImage(false);
      ImageFileBlockInfo[(int)ImageFileBlock.Exif] = ImageFileBlockState.Removed;
    }


    public void RemoveImageFileBlock(ImageFileBlock BlockType)
    {
      if (BlockType == ImageFileBlock.Exif)
      {
        RemoveAllTags();
      }
      else if (BlockType != ImageFileBlock.Unknown)
      {
        if (ImageType == ImageType.Tiff)
        {
          if (BlockType == ImageFileBlock.Xmp)
          {
            RemoveTag(ExifTag.XmpMetadata);

          }
          else if (BlockType == ImageFileBlock.Iptc)
          {
            RemoveTag(ExifTag.IptcMetadata);
          }
        }
        ImageFileBlockInfo[(int)BlockType] = ImageFileBlockState.Removed;
      }
    }


    // Replace all EXIF tags and the thumbnail image by the EXIF data of another image file.
    //
    // Parameters:
    //  "SourceExifData": EXIF data of another image, that will be copied to the EXIF data of the current object.
    //                    A deep copy is done.
    public void ReplaceAllTagsBy(ExifData SourceExifData)
    {
      // Byte order handling
      bool SwapByteOrder = false;
      if (this.ImageType == ImageType.Tiff)
      {
        if (this._ByteOrder != SourceExifData._ByteOrder)
        {
          // In TIFF images the EXIF block cannot be completely replaced by another EXIF block because it contains
          // internal tags which must not be changed. If the byte order of the two EXIF blocks is different, the byte order
          // of the source EXIF block shall be adapted.
          SwapByteOrder = true;
        }
      }
      else this._ByteOrder = SourceExifData._ByteOrder; // Complete EXIF block is copied, therefore the byte order is also copied.

      // Remove all tags. But in TIFF images the TIFF internal tags, the XMP tag and the IPTC tag are not removed.
      this.RemoveAllTags();

      // Copy IFD Primary data.
      Dictionary<ExifTagId, TagItem> SourceIfdTagList = SourceExifData.TagTable[(uint)ExifIfd.PrimaryData];
      Dictionary<ExifTagId, TagItem> DestIfdTagList = this.TagTable[(uint)ExifIfd.PrimaryData];
      bool IsTiffImagePresent = (this.ImageType == ImageType.Tiff) || (SourceExifData.ImageType == ImageType.Tiff);
      bool CopyTag = true;
      foreach (TagItem SourceTag in SourceIfdTagList.Values)
      {
        if (IsTiffImagePresent)
        {
          CopyTag = !IsInternalTiffTag(SourceTag.TagId) && !IsMetaDataTiffTag(SourceTag.TagId);
        }
        if (CopyTag)
        {
          TagItem DestTag = CopyTagItemDeeply(ExifIfd.PrimaryData, SourceTag, SwapByteOrder);
          AddIfNotExists(DestIfdTagList, DestTag);
        }
      }

      // Copy IFDs PrivateData, GpsInfoData and Interoperability
      ExifIfd[] TempIfds = new ExifIfd[] { ExifIfd.PrivateData, ExifIfd.GpsInfoData, ExifIfd.Interoperability };
      foreach (ExifIfd Ifd in TempIfds)
      {
        SourceIfdTagList = SourceExifData.TagTable[(uint)Ifd];
        DestIfdTagList = this.TagTable[(uint)Ifd];
        foreach (TagItem SourceTag in SourceIfdTagList.Values)
        {
          TagItem DestTag = CopyTagItemDeeply(Ifd, SourceTag, SwapByteOrder);
          AddIfNotExists(DestIfdTagList, DestTag);
        }
      }

      // Copy IFD ThumbnailData
      if (this.ImageType == ImageType.Jpeg)
      {
        SourceIfdTagList = SourceExifData.TagTable[(uint)ExifIfd.ThumbnailData];
        DestIfdTagList = this.TagTable[(uint)ExifIfd.ThumbnailData];
        foreach (TagItem SourceTag in SourceIfdTagList.Values)
        {
          TagItem DestTag = CopyTagItemDeeply(ExifIfd.ThumbnailData, SourceTag, SwapByteOrder);
          AddIfNotExists(DestIfdTagList, DestTag);
        }

        if (SourceExifData.ThumbnailImageExists())
        {
          this.ThumbnailStartIndex = 0;
          this.ThumbnailByteCount = SourceExifData.ThumbnailByteCount;
          this.ThumbnailImage = new byte[this.ThumbnailByteCount];
          Array.Copy(SourceExifData.ThumbnailImage, SourceExifData.ThumbnailStartIndex, this.ThumbnailImage, 0, this.ThumbnailByteCount);
        }
        else
        {
          RemoveThumbnailImage_Internal();
        }
      }

      // Determine new EXIF block state
      UpdateImageFileBlockInfo();

      // Copy MakerNote index
      this.MakerNoteOriginalOffset = SourceExifData.MakerNoteOriginalOffset;
    }


    public bool ThumbnailImageExists()
    {
      return (ThumbnailImage != null);
    }


    // Read the thumbnail image data.
    //
    // Parameters:
    //  "ThumbnailData":      Return: Array with the thumbnail image beginning with the JPEG marker 0xFF 0xD8.
    //                        If no thumbnail image is defined, "null" is returned.
    //  "ThumbnailIndex":     Return: Index of the first data byte of the thumbnail image.
    //  "ThumbnailByteCount": Return: Number of bytes of the thumbnail image.
    //  Return value:         false = Thumbnail image does not exist.
    //                        true  = Thumbnail image is given back.
    public bool GetThumbnailImage(out byte[] ThumbnailData, out int ThumbnailIndex, out int ThumbnailByteCount)
    {
      bool Success;

      ThumbnailData = ThumbnailImage;
      ThumbnailIndex = ThumbnailStartIndex;
      ThumbnailByteCount = this.ThumbnailByteCount;
      Success = ThumbnailImageExists();
      return (Success);
    }


    // Write the thumbnail image data.
    //
    // Parameters:
    //  "ThumbnailData": Array which contains the thumbnail image beginning with the JPEG marker 0xFF 0xD8.
    //                   The array is not copied by this method so it should not be changed after calling
    //                   this method.
    public bool SetThumbnailImage(byte[] ThumbnailData, int ThumbnailIndex = 0, int ThumbnailByteCount = -1)
    {
      bool Success = false;

      if (ThumbnailData != null)
      {
        ThumbnailImage = ThumbnailData;
        ThumbnailStartIndex = ThumbnailIndex;
        if (ThumbnailByteCount < 0)
        {
          this.ThumbnailByteCount = ThumbnailData.Length - ThumbnailIndex;
        }
        else this.ThumbnailByteCount = ThumbnailByteCount;
        SetTagValue(ExifTag.JpegInterchangeFormat, 0, ExifTagType.ULong); // "0" is a dummy value
        SetTagValue(ExifTag.JpegInterchangeFormatLength, this.ThumbnailByteCount, ExifTagType.ULong);
        Success = true;
      }
      return (Success);
    }


    // Remove the thumbnail image data.
    //
    // Parameters:
    //  "RemoveAlsoThumbnailTags": false = Remove the thumbnail image, the thumbnail pointer tag and the thumbnail size tag.
    //                             true  = Remove also all other tags in the IFD "thumbnail data".
    public void RemoveThumbnailImage(bool RemoveAlsoThumbnailTags)
    {
      RemoveThumbnailImage_Internal();
      if (RemoveAlsoThumbnailTags)
      {
        ClearIfd_Unchecked(ExifIfd.ThumbnailData);
      }
      else
      {
        RemoveTag(ExifTag.JpegInterchangeFormat);
        RemoveTag(ExifTag.JpegInterchangeFormatLength);
      }
    }


    // Get the byte order for the EXIF data.
    public ExifByteOrder ByteOrder
    {
      get
      {
        return (_ByteOrder);
      }
    }


    // Extract the IFD of a tag specification.
    public static ExifIfd ExtractIfd(ExifTag TagSpec)
    {
      return (ExifIfd)((uint)TagSpec >> IfdShift);
    }


    // Extract the tag ID of a tag specification.
    public static ExifTagId ExtractTagId(ExifTag TagSpec)
    {
      return (ExifTagId)((ushort)TagSpec);
    }


    // Compose IFD and tag ID to a tag specification.
    public static ExifTag ComposeTagSpec(ExifIfd Ifd, ExifTagId TagId)
    {
      return (ExifTag)(((uint)Ifd << IfdShift) | (uint)TagId);
    }


    // Get the number of bytes that a tag with the specified type and value count has.
    // If the tag type is invalid, the return value is 0.
    public static int GetTagByteCount(ExifTagType TagType, int ValueCount)
    {
      if ((uint)TagType < TypeByteCountLen)
      {
        return (TypeByteCount[(uint)TagType] * ValueCount);
      }
      else return (0);
    }


    public ushort ExifReadUInt16(byte[] Data, int StartIndex)
    {
      ushort v;

      if (_ByteOrder == ExifByteOrder.BigEndian)
      {
        v = (ushort)(((uint)Data[StartIndex] << 8) | Data[StartIndex + 1]);
      }
      else
      {
        v = (ushort)(((uint)Data[StartIndex + 1] << 8) | Data[StartIndex]);
      }
      return (v);
    }


    public void ExifWriteUInt16(byte[] Data, int StartIndex, ushort Value)
    {
      if (_ByteOrder == ExifByteOrder.BigEndian)
      {
        Data[StartIndex] = (byte)(Value >> 8);
        Data[StartIndex + 1] = (byte)Value;
      }
      else
      {
        Data[StartIndex + 1] = (byte)(Value >> 8);
        Data[StartIndex] = (byte)Value;
      }
    }


    public uint ExifReadUInt32(byte[] Data, int StartIndex)
    {
      uint v;

      if (_ByteOrder == ExifByteOrder.BigEndian)
      {
        v = ((uint)Data[StartIndex] << 24) |
            ((uint)Data[StartIndex + 1] << 16) |
            ((uint)Data[StartIndex + 2] << 8) |
            (Data[StartIndex + 3]);
      }
      else
      {
        v = ((uint)Data[StartIndex + 3] << 24) |
           ((uint)Data[StartIndex + 2] << 16) |
           ((uint)Data[StartIndex + 1] << 8) |
           (Data[StartIndex]);
      }
      return (v);
    }


    public void ExifWriteUInt32(byte[] Data, int StartIndex, uint Value)
    {
      if (_ByteOrder == ExifByteOrder.BigEndian)
      {
        Data[StartIndex] = (byte)(Value >> 24);
        Data[StartIndex + 1] = (byte)(Value >> 16);
        Data[StartIndex + 2] = (byte)(Value >> 8);
        Data[StartIndex + 3] = (byte)(Value);
      }
      else
      {
        Data[StartIndex + 3] = (byte)(Value >> 24);
        Data[StartIndex + 2] = (byte)(Value >> 16);
        Data[StartIndex + 1] = (byte)(Value >> 8);
        Data[StartIndex] = (byte)(Value);
      }
    }


    private ushort ReadUInt16BE(byte[] Data, int StartIndex)
    {
      ushort v = (ushort)(((uint)Data[StartIndex] << 8) | Data[StartIndex + 1]);
      return (v);
    }


    private uint ReadUInt32BE(byte[] Data, int StartIndex)
    {
      uint v = ((uint)Data[StartIndex] << 24) |
          ((uint)Data[StartIndex + 1] << 16) |
          ((uint)Data[StartIndex + 2] << 8) |
          (Data[StartIndex + 3]);
      return (v);
    }


    private void WriteUInt16BE(byte[] Data, int StartIndex, ushort Value)
    {
        Data[StartIndex] = (byte)(Value >> 8);
        Data[StartIndex + 1] = (byte)Value;
    }


    private void WriteUInt32BE(byte[] Data, int StartIndex, uint Value)
    {
      Data[StartIndex] = (byte)(Value >> 24);
      Data[StartIndex + 1] = (byte)(Value >> 16);
      Data[StartIndex + 2] = (byte)(Value >> 8);
      Data[StartIndex + 3] = (byte)(Value);
    }


    // Initialize enumeration for all tags of a specific IFD.
    public bool InitTagEnumeration(ExifIfd Ifd)
    {
      if ((uint)Ifd < ExifIfdCount)
      {
        ExifIfdForTagEnumeration = Ifd;
        TagEnumerator = TagTable[(uint)Ifd].Keys.GetEnumerator();
        return (true);
      }
      else
      {
        TagEnumerator = new Dictionary<ExifTagId, TagItem>.KeyCollection.Enumerator();
        return (false);
      }
    }


    // Get the next tag of the IFD, that was defined by a previous call to the method "InitTagEnumeration".
    //
    // Return value: true  = Next tag was successfully read.
    //               false = There is no further tag available.
    public bool EnumerateNextTag(out ExifTag TagSpec)
    {
      bool Success = false;

      if (TagEnumerator.MoveNext())
      {
        ExifTagId TagId = TagEnumerator.Current;
        TagSpec = ComposeTagSpec(ExifIfdForTagEnumeration, TagId);
        Success = true;
      }
      else TagSpec = 0;
      return (Success);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods for accessing EXIF tags on high level
    // -----------------------------------------------------------------------------------------------------------------

    public bool GetDateTaken(out DateTime Value)
    {
      return (GetDateAndTimeWithMillisecHelper(out Value, ExifTag.DateTimeOriginal, ExifTag.SubsecTimeOriginal));
    }


    public bool SetDateTaken(DateTime Value)
    {
      return (SetDateAndTimeWithMillisecHelper(Value, ExifTag.DateTimeOriginal, ExifTag.SubsecTimeOriginal));
    }


    public void RemoveDateTaken()
    {
      RemoveTag(ExifTag.DateTimeOriginal);
      RemoveTag(ExifTag.SubsecTimeOriginal);
    }


    public bool GetDateDigitized(out DateTime Value)
    {
      return (GetDateAndTimeWithMillisecHelper(out Value, ExifTag.DateTimeDigitized, ExifTag.SubsecTimeDigitized));
    }


    public bool SetDateDigitized(DateTime Value)
    {
      return (SetDateAndTimeWithMillisecHelper(Value, ExifTag.DateTimeDigitized, ExifTag.SubsecTimeDigitized));
    }


    public void RemoveDateDigitized()
    {
      RemoveTag(ExifTag.DateTimeDigitized);
      RemoveTag(ExifTag.SubsecTimeDigitized);
    }


    public bool GetDateChanged(out DateTime Value)
    {
      return (GetDateAndTimeWithMillisecHelper(out Value, ExifTag.DateTime, ExifTag.SubsecTime));
    }


    public bool SetDateChanged(DateTime Value)
    {
      return (SetDateAndTimeWithMillisecHelper(Value, ExifTag.DateTime, ExifTag.SubsecTime));
    }


    public void RemoveDateChanged()
    {
      RemoveTag(ExifTag.DateTime);
      RemoveTag(ExifTag.SubsecTime);
    }


    public bool GetGpsLongitude(out GeoCoordinate Value)
    {
      return (GetGpsCoordinateHelper(out Value, ExifTag.GpsLongitude, ExifTag.GpsLongitudeRef, 'W', 'E'));
    }


    public bool SetGpsLongitude(GeoCoordinate Value)
    {
      return (SetGpsCoordinateHelper(Value, ExifTag.GpsLongitude, ExifTag.GpsLongitudeRef, 'W', 'E'));
    }


    public void RemoveGpsLongitude()
    {
      RemoveTag(ExifTag.GpsLongitude);
      RemoveTag(ExifTag.GpsLongitudeRef);
    }


    public bool GetGpsLatitude(out GeoCoordinate Value)
    {
      return (GetGpsCoordinateHelper(out Value, ExifTag.GpsLatitude, ExifTag.GpsLatitudeRef, 'N', 'S'));
    }


    public bool SetGpsLatitude(GeoCoordinate Value)
    {
      return (SetGpsCoordinateHelper(Value, ExifTag.GpsLatitude, ExifTag.GpsLatitudeRef, 'N', 'S'));
    }


    public void RemoveGpsLatitude()
    {
      RemoveTag(ExifTag.GpsLatitude);
      RemoveTag(ExifTag.GpsLatitudeRef);
    }


    public bool GetGpsAltitude(out decimal Value)
    {
      bool Success = false;

            if (GetTagValue(ExifTag.GpsAltitude, out ExifRational AltitudeRat) && AltitudeRat.IsValid())
            {
                Value = ExifRational.ToDecimal(AltitudeRat);
                if (GetTagValue(ExifTag.GpsAltitudeRef, out uint BelowSeaLevel) && (BelowSeaLevel == 1))
                {
                    Value = -Value;
                }
                Success = true;
            }
            else Value = 0;
            return (Success);
    }


    public bool SetGpsAltitude(decimal Value)
    {
      bool Success = false;
      ExifRational AltitudeRat = ExifRational.FromDecimal(Value);
      uint BelowSeaLevel = 0;
      if (AltitudeRat.IsNegative())
      {
        BelowSeaLevel = 1;
        AltitudeRat.Sign = false; // Remove negative sign from "AltitudeRat"
      }
      if (SetTagValue(ExifTag.GpsAltitude, AltitudeRat, ExifTagType.URational) &&
          SetTagValue(ExifTag.GpsAltitudeRef, BelowSeaLevel, ExifTagType.Byte))
      {
        Success = true;
      }
      return (Success);
    }


    public void RemoveGpsAltitude()
    {
      RemoveTag(ExifTag.GpsAltitude);
      RemoveTag(ExifTag.GpsAltitudeRef);
    }


    public bool GetGpsDateTimeStamp(out DateTime Value)
    {
      bool Success = false;

            if (GetTagValue(ExifTag.GpsDateStamp, out Value, ExifDateFormat.DateOnly))
            {
                if (GetTagValue(ExifTag.GpsTimeStamp, out ExifRational Hour, 0) && !Hour.IsNegative() && Hour.IsValid() &&
                    GetTagValue(ExifTag.GpsTimeStamp, out ExifRational Min, 1) && !Min.IsNegative() && Min.IsValid() &&
                    GetTagValue(ExifTag.GpsTimeStamp, out ExifRational Sec, 2) && !Sec.IsNegative() && Sec.IsValid())
                {
                    Value = Value.AddHours(((double)Hour.Numer) / Hour.Denom);
                    Value = Value.AddMinutes(((double)Min.Numer) / Min.Denom);
                    double ms = Math.Truncate(((double)Sec.Numer * 1000) / Sec.Denom);
                    Value = Value.AddMilliseconds(ms);
                    Success = true;
                }
                else Value = DateTime.MinValue;
            }
            else Value = DateTime.MinValue;
            return (Success);
    }


    public bool SetGpsDateTimeStamp(DateTime Value)
    {
      bool Success = false;
      ExifRational Sec;

      if (SetTagValue(ExifTag.GpsDateStamp, Value.Date, ExifDateFormat.DateOnly))
      {
        TimeSpan ts = Value.TimeOfDay;
        ExifRational Hour = new ExifRational(ts.Hours, 1);
        ExifRational Min = new ExifRational(ts.Minutes, 1);
        int ms = ts.Milliseconds;
        if (ms == 0)
        {
          Sec = new ExifRational(ts.Seconds, 1);
        }
        else
        {
          Sec = new ExifRational(ts.Seconds * 1000 + ms, 1000);
        }
        if (SetTagValue(ExifTag.GpsTimeStamp, Hour, ExifTagType.URational, 0) &&
            SetTagValue(ExifTag.GpsTimeStamp, Min, ExifTagType.URational, 1) &&
            SetTagValue(ExifTag.GpsTimeStamp, Sec, ExifTagType.URational, 2))
        {
          Success = true;
        }
      }
      return (Success);
    }


    public void RemoveGpsDateTimeStamp()
    {
      RemoveTag(ExifTag.GpsDateStamp);
      RemoveTag(ExifTag.GpsTimeStamp);
    }

    #endregion

    #region Private area
    // -----------------------------------------------------------------------------------------------------------------
    // --- Private area ------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------------------
    private const int MinExifBlockLen = 2 + 4; // Minimum number of bytes for an EXIF block: Value for the number of tags of IFD Primary Data (2 bytes) +
                                               // Offset of IFD Thumbnail Data (JPEG) or offset of next EXIF block (TIFF) (4 bytes)
    private const int MaxTagValueCount = int.MaxValue / 8;
    private byte[] SourceExifBlock; // Temporary storage for the EXIF block in JPEG and PNG images, starting at index 8.
                                    // In index 0 to 7 the TIFF header is stored.
    private Stream SourceExifStream; // Temporary stream from which the EXIF block is read in TIFF images.
    private ExifByteOrder _ByteOrder; // Byte order value, extracted from the TIFF header
    private string _FileNameWithPath;
    private static readonly int[] TypeByteCount = {
      0, // Type 0: invalid
      1, // Type 1: ExifTagType.Byte
      1, // Type 2: ExifTagType.Ascii, 8 bit character
      2, // Type 3: ExifTagType.UShort
      4, // Type 4: ExifTagType.ULong
      8, // Type 5: ExifTagType.URational
      1, // Type 6: ExifTagType.SByte
      1, // Type 7: ExifTagType.Undefined, 8 bit value
      2, // Type 8: ExifTagType.SShort
      4, // Type 9: ExifTagType.SLong
      8, // Type 10: ExifTagType.SRational
      4, // Type 11: ExifTagType.Float
      8  // Type 12: ExifTagType.Double
    };
    private const int TypeByteCountLen = 13; // Number of elements of the array "TypeByteCount"
    private const int IdCodeLength = 8;
    private static readonly byte[] IdCodeUtf16 = new byte[] { (byte)'U', (byte)'N', (byte)'I', (byte)'C', (byte)'O', (byte)'D', (byte)'E', 0 };
    private static readonly byte[] IdCodeAscii = new byte[] { (byte)'A', (byte)'S', (byte)'C', (byte)'I', (byte)'I', 0, 0, 0 };
    private static readonly byte[] IdCodeDefault = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

    private Dictionary<ExifTagId, TagItem>[] TagTable;
    private byte[] ThumbnailImage; // Array, which contains among other data the thumbnail image
    private int ThumbnailStartIndex, ThumbnailByteCount; // Range, in which the thumbnail image in the array "ThumbnailData" is stored

    private const int ExifIfdCount = 5; // Number of elements of the enum type "ExifIfd", i. e. the number of IFDs
    private ExifIfd ExifIfdForTagEnumeration;
    private Dictionary<ExifTagId, TagItem>.KeyCollection.Enumerator TagEnumerator;
    private enum ImageFileBlockState { NonExistent = 0, Removed, Existent };
    //private const ImageFileBlock LastImageFileBlock = ImageFileBlock.JpegComment;
    private ImageFileBlockState[] ImageFileBlockInfo;
    private ExifErrCode ErrCodeForIllegalExifBlock;

    //private int ExifBlockFilePosition = 0;

    // JPEG files
    private const int JpegMaxExifBlockLen = 65534 - 2 - 6 - TiffHeaderLen; // Maximum number of bytes an EXIF block in a JPEG file may have
    private const ushort JpegApp0Marker = 0xFFE0;
    private const ushort JpegApp1Marker = 0xFFE1;
    private const ushort JpegApp2Marker = 0xFFE2;
    private const ushort JpegApp13Marker = 0xFFED;
    private const ushort JpegApp14Marker = 0xFFEE;
    private const ushort JpegCommentMarker = 0xFFFE; // Start of JPEG comment block
    private const ushort JpegSoiMarker = 0xFFD8; // Start of image (SOI) marker
    private const ushort JpegSosMarker = 0xFFDA; // Start of scan (SOS) marker
    private static readonly byte[] JpegExifSignature = { (byte)'E', (byte)'x', (byte)'i', (byte)'f', 0, 0 };
    private static readonly byte[] JpegXmpInitialSignature = { (byte)'h', (byte)'t', (byte)'t', (byte)'p', (byte)':',
      (byte)'/', (byte)'/', (byte)'n', (byte)'s', (byte)'.', (byte)'a', (byte)'d', (byte)'o', (byte)'b',
      (byte)'e', (byte)'.', (byte)'c', (byte)'o', (byte)'m', (byte)'/' };
    private static readonly byte[] JpegIptcSignature = { (byte)'P', (byte)'h', (byte)'o', (byte)'t', (byte)'o',
      (byte)'s', (byte)'h', (byte)'o', (byte)'p', (byte)' ', (byte)'3', (byte)'.', (byte)'0', 0 };

    // TIFF files
    private const int TiffHeaderLen = 8; // Size of TIFF header. The TIFF header is also present in EXIF blocks of JPEG and PNG files.
    private const uint TiffHeaderSignatureLE = 0x49492A00;
    private const uint TiffHeaderSignatureBE = 0x4D4D002A;

    // PNG files
    private const uint PngHeaderPart1 = 0x89504E47;
    private const uint PngHeaderPart2 = 0x0D0A1A0A;
    private const uint PngIhdrChunk = ((uint)'I' << 24) | ((uint)'H' << 16) | ((uint)'D' << 8) | (uint)'R';
    private const uint PngExifChunk = ((uint)'e' << 24) | ((uint)'X' << 16) | ((uint)'I' << 8) | (uint)'f';
    private const uint PngItxtChunk = ((uint)'i' << 24) | ((uint)'T' << 16) | ((uint)'X' << 8) | (uint)'t';
    private const uint PngTextChunk = ((uint)'t' << 24) | ((uint)'E' << 16) | ((uint)'X' << 8) | (uint)'t';
    private const uint PngTimeChunk = ((uint)'t' << 24) | ((uint)'I' << 16) | ((uint)'M' << 8) | (uint)'E';
    private const uint PngIendChunk = ((uint)'I' << 24) | ((uint)'E' << 16) | ((uint)'N' << 8) | (uint)'D';
    private static readonly byte[] PngXmpSignature = { (byte)'X', (byte)'M', (byte)'L', (byte)':', (byte)'c',
      (byte)'o', (byte)'m', (byte)'.', (byte)'a', (byte)'d', (byte)'o', (byte)'b', (byte)'e', (byte)'.',
      (byte)'x', (byte)'m', (byte)'p', 0 };
    private static readonly byte[] PngIptcSignature = { (byte)'R', (byte)'a', (byte)'w', (byte)' ', (byte)'p',
      (byte)'r', (byte)'o', (byte)'f', (byte)'i', (byte)'l', (byte)'e', (byte)' ', (byte)'t', (byte)'y',
      (byte)'p', (byte)'e', (byte)' ', (byte)'i', (byte)'p', (byte)'t', (byte)'c', 0 };
    private static readonly int ImageFileBlockCount = Enum.GetValues(typeof(ImageFileBlock)).Length;


    private ExifData()
    {
      _ByteOrder = ExifByteOrder.BigEndian;
      ImageFileBlockInfo = new ImageFileBlockState[ImageFileBlockCount];
    }


    private void ReadFromStream(Stream ImageStream, ExifLoadOptions Options)
    {
      SourceExifStream = ImageStream;
      ErrCodeForIllegalExifBlock = ExifErrCode.ExifBlockHasIllegalContent;
      ImageType = CheckStreamTypeAndCompatibility(SourceExifStream);
      if (Options.HasFlag(ExifLoadOptions.CreateEmptyBlock))
      {
        SetEmptyExifBlock();
      }
      else
      {
        if (ImageType == ImageType.Jpeg)
        {
          ReadJepg();
        }
        else if (ImageType == ImageType.Tiff)
        {
          ReadTiff();
        }
        else if (ImageType == ImageType.Png)
        {
          ReadPng();
        }
        UpdateImageFileBlockInfo();
      }

      SourceExifBlock = null;
      SourceExifStream = null;
    }


    // Read JPEG file from stream "SourceExifStream".
    private void ReadJepg()
    {
      byte[] BlockContent = new byte[65536];
      ushort BlockMarker;
            bool ExifBlockFound = false;

            SourceExifStream.Position = 2;
      do
      {
        ReadJpegBlock(SourceExifStream, BlockContent, out int BlockContentSize, out BlockMarker, out ImageFileBlock BlockType, out int MetaDataIndex);
        if (BlockType != ImageFileBlock.Unknown)
        {
          if ((BlockType == ImageFileBlock.Exif) && (!ExifBlockFound))
          {
            ExifBlockFound = true;
            int TiffHeaderAndExifBlockLen = BlockContentSize - MetaDataIndex;
            SourceExifBlock = new byte[TiffHeaderAndExifBlockLen];
            Array.Copy(BlockContent, MetaDataIndex, SourceExifBlock, 0, TiffHeaderAndExifBlockLen);
            EvaluateTiffHeader(SourceExifBlock, TiffHeaderAndExifBlockLen, out int IfdPrimaryDataOffset);
            EvaluateExifBlock(IfdPrimaryDataOffset);
          }
          ImageFileBlockInfo[(int)BlockType] = ImageFileBlockState.Existent;
        }
      } while (BlockMarker != JpegSosMarker);
      if (!ExifBlockFound)
      {
        SetEmptyExifBlock();
      }
    }


    // Read TIFF file from stream "SourceExifStream".
    private void ReadTiff()
    {
      byte[] BlockContent = new byte[8];

      SourceExifStream.Position = 0;
      SourceExifBlock = null;
      ErrCodeForIllegalExifBlock = ExifErrCode.InternalImageStructureIsWrong;
      int TiffHeaderBytesRead = SourceExifStream.Read(BlockContent, 0, TiffHeaderLen);
      EvaluateTiffHeader(BlockContent, TiffHeaderBytesRead, out int IfdPrimaryDataOffset);
      EvaluateExifBlock(IfdPrimaryDataOffset);
    }


    // Read PNG file from stream "SourceExifStream".
    private void ReadPng()
    {
      byte[] TempData = new byte[65536];
      byte[] BlockContent = new byte[30];
      bool ExifBlockFound = false;
      uint ChunkType;

      SourceExifStream.Position = 4;
      int k = SourceExifStream.Read(TempData, 0, 4);
      if ((k < 4) || (ReadUInt32BE(TempData, 0) != PngHeaderPart2))
      {
        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
      }
      do
      {
        ReadPngBlockHeader(SourceExifStream, TempData, out int DataLength, out ChunkType);
        int BytesRead = DetectPngImageBlock(SourceExifStream, BlockContent, ChunkType, out ImageFileBlock BlockType);
        if (BlockType != ImageFileBlock.Unknown)
        {
          if ((BlockType == ImageFileBlock.Exif) && (!ExifBlockFound))
          {
            ExifBlockFound = true;
            SourceExifBlock = new byte[DataLength];
            if (SourceExifStream.Read(SourceExifBlock, 0, DataLength) < DataLength)
            {
              throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
            }
            uint Crc32Calculated = CalculateCrc32(TempData, 4, 4, false); // Calculate CRC32 of chunk type
            Crc32Calculated = CalculateCrc32(SourceExifBlock, 0, DataLength, true, Crc32Calculated);
            k = SourceExifStream.Read(TempData, 0, 4);
            uint Crc32Loaded = ReadUInt32BE(TempData, 0);
            if ((k < 4) || (Crc32Calculated != Crc32Loaded))
            {
              throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
            }
            EvaluateTiffHeader(SourceExifBlock, DataLength, out int IfdPrimaryDataOffset);
            EvaluateExifBlock(IfdPrimaryDataOffset);
            BytesRead = DataLength + 4;
          }
          ImageFileBlockInfo[(int)BlockType] = ImageFileBlockState.Existent;
        }
        SourceExifStream.Position += DataLength + 4 - BytesRead;
      } while (ChunkType != PngIendChunk);
      if (!ExifBlockFound)
      {
        SetEmptyExifBlock();
      }
    }


    // Read block length and chunk type and return them in "TempData" as raw data and in "BlockLength" and
    // "ChunkType" as 32 bit values.
    private void ReadPngBlockHeader(Stream ImageStream, byte[] TempData, out int BlockLength, out uint ChunkType)
    {
      if (ImageStream.Read(TempData, 0, 8) < 8)
      {
        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
      }
      BlockLength = (int)ReadUInt32BE(TempData, 0);
      if (BlockLength < 0)
      {
        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
      }
      ChunkType = ReadUInt32BE(TempData, 4);
    }


    private int DetectPngImageBlock(Stream ImageStream, byte[] SignatureData, uint ChunkType, out ImageFileBlock BlockType)
    {
      int BytesRead = 0;
      BlockType = ImageFileBlock.Unknown;
      if (ChunkType == PngExifChunk)
      {
        BlockType = ImageFileBlock.Exif;
      }
      else if (ChunkType == PngItxtChunk)
      {
        int BytesToRead = PngIptcSignature.Length;
        BytesRead = ImageStream.Read(SignatureData, 0, BytesToRead);
        if (ArrayStartsWith(SignatureData, BytesRead, PngXmpSignature))
        {
          BlockType = ImageFileBlock.Xmp;
        }
        else if (ArrayStartsWith(SignatureData, BytesRead, PngIptcSignature))
        {
          BlockType = ImageFileBlock.Iptc;
        }
      }
      else if (ChunkType == PngTextChunk)
      {
        BlockType = ImageFileBlock.PngMetaData;
      }
      else if (ChunkType == PngTimeChunk)
      {
        BlockType = ImageFileBlock.PngDateChanged;
      }
      return (BytesRead);
    }


    // CRC32 checksum table for polynomial 0xEDB88320
    private static readonly uint[] Crc32ChecksumTable = { 0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F,
      0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
      0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856, 0x646BA8C0,
      0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172,
      0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B, 0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75,
      0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
      0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190, 0x01DB7106,
      0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818,
      0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01, 0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B,
      0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
      0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A, 0x346ED9FC,
      0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086,
      0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F, 0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81,
      0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
      0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344, 0x8708A3D2, 
      0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC, 
      0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5, 0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 
      0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79, 
      0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE, 0xB2BD0B28,
      0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 
      0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713, 0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D, 
      0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777, 
      0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278, 0xD70DD2EE, 
      0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0, 
      0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9, 0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693, 
      0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D };


    private static uint CalculateCrc32(byte[] Data, int StartIndex, int Length, bool Finalize = true, uint StartCrc = 0xFFFFFFFF)
    {
      uint result = StartCrc;
      int EndIndexPlus1 = StartIndex + Length;
      for (int i = StartIndex; i < EndIndexPlus1; i++)
      {
        byte value = Data[i];
        result = Crc32ChecksumTable[((byte)result) ^ value] ^ (result >> 8);
      }
      if (Finalize) result = ~result;
      return (result);
    }


    private void UpdateImageFileBlockInfo()
    {
      ImageFileBlockState NewExifBlockState = ImageFileBlockState.NonExistent;
      Dictionary<ExifTagId, TagItem> PrimaryDataIfdTable = TagTable[(uint)ExifIfd.PrimaryData];
      foreach (ExifTagId tid in PrimaryDataIfdTable.Keys)
      {
        if (ImageType == ImageType.Tiff)
        {
          if (!IsInternalTiffTag(tid) && !IsMetaDataTiffTag(tid))
          {
            NewExifBlockState = ImageFileBlockState.Existent;
            break;
          }
        }
        else
        {
          NewExifBlockState = ImageFileBlockState.Existent;
          break;
        }
      }
      
      if (NewExifBlockState == ImageFileBlockState.NonExistent)
      {
        ExifIfd[] TempIfds = new ExifIfd[] { ExifIfd.PrivateData, ExifIfd.GpsInfoData, ExifIfd.Interoperability, ExifIfd.ThumbnailData };
        foreach (ExifIfd Ifd in TempIfds)
        {
          if (this.TagTable[(uint)Ifd].Count > 0)
          {
            NewExifBlockState = ImageFileBlockState.Existent;
            break;
          }
        }
      }
      ImageFileBlockInfo[(int)ImageFileBlock.Exif] = NewExifBlockState;

      if (ImageType == ImageType.Tiff)
      {
        // In TIFF images the XMP and IPTC block are stored as tags within the EXIF block
        if (PrimaryDataIfdTable.ContainsKey(ExifTagId.XmpMetadata))
        {
          ImageFileBlockInfo[(int)ImageFileBlock.Xmp] = ImageFileBlockState.Existent;
        }
        else ImageFileBlockInfo[(int)ImageFileBlock.Xmp] = ImageFileBlockState.NonExistent;
        if (PrimaryDataIfdTable.ContainsKey(ExifTagId.IptcMetadata))
        {
          ImageFileBlockInfo[(int)ImageFileBlock.Iptc] = ImageFileBlockState.Existent;
        }
        else ImageFileBlockInfo[(int)ImageFileBlock.Iptc] = ImageFileBlockState.NonExistent;
      }
    }


    private void ReadJpegBlockMarker(Stream ImageStream, out ushort BlockMarker, out int BlockContentSize)
    {
      byte[] TempBuffer = new byte[2];

      ImageStream.Read(TempBuffer, 0, 2);
      BlockMarker = ReadUInt16BE(TempBuffer, 0);
      if ((BlockMarker == 0xFF01) || ((BlockMarker >= 0xFFD0) && (BlockMarker <= 0xFFDA)))
      {
        // Block does not have a size specification
        BlockContentSize = 0;
      }
      else if (BlockMarker == 0xFFFF)
      {
        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
      }
      else
      {
        ImageStream.Read(TempBuffer, 0, 2);
        BlockContentSize = (int)ReadUInt16BE(TempBuffer, 0) - 2;
        if (BlockContentSize < 0)
        {
          throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
        }
      }
    }


    // Read JPEG block. If the JPEG block has a size specification, the block content is read and returned.
    // If the JPEG block doesn't have a size specification, only the block marker is read and "BlockContentSize" is set to 0.
    // 
    // Parameters:
    //  "BlockData": in:  Allocated array with a size of at least 65535 Bytes.
    //               out: Content of JPEG block.
    private void ReadJpegBlock(Stream ImageStream, byte[] BlockContent, out int BlockContentSize, out ushort BlockMarker,
      out ImageFileBlock BlockType, out int MetaDataIndex)
    {
      BlockType = ImageFileBlock.Unknown;
      MetaDataIndex = 0;
      ReadJpegBlockMarker(ImageStream, out BlockMarker, out int ContentSize);
      if (ContentSize > 0)
      {
        int k = ImageStream.Read(BlockContent, 0, ContentSize);
        if (k == ContentSize)
        {
          if (BlockMarker == JpegApp1Marker)
          {
            if (ArrayStartsWith(BlockContent, ContentSize, JpegExifSignature))
            {
              BlockType = ImageFileBlock.Exif;
              MetaDataIndex = JpegExifSignature.Length;
            }
            else if (ArrayStartsWith(BlockContent, ContentSize, JpegXmpInitialSignature))
            {
              BlockType = ImageFileBlock.Xmp;
              int i = JpegXmpInitialSignature.Length;
              while ((i < ContentSize) && (BlockContent[i] != 0))
              {
                i++;
              }
              MetaDataIndex = i + 1;
            }
          }
          else if (BlockMarker == JpegApp13Marker)
          {
            if (ArrayStartsWith(BlockContent, ContentSize, JpegIptcSignature))
            {
              BlockType = ImageFileBlock.Iptc;
              MetaDataIndex = JpegIptcSignature.Length;
            }
          }
          else if (BlockMarker == JpegCommentMarker)
          {
            BlockType = ImageFileBlock.JpegComment;
          }
        }
        else throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
      }
      BlockContentSize = ContentSize;
    }


    // The first 4 bytes are read from the stream "StreamToBeChecked" to identify the image type.
    private ImageType CheckStreamTypeAndCompatibility(Stream StreamToBeChecked)
    {
      ImageType StreamImageType;
      byte[] TempBuffer = new byte[4];

      StreamToBeChecked.Read(TempBuffer, 0, 4);
      uint ImageSignature = ReadUInt32BE(TempBuffer, 0);
      if ((ImageSignature >> 16) == JpegSoiMarker)
      {
        StreamImageType = ImageType.Jpeg;
      }
      else if ((ImageSignature == TiffHeaderSignatureLE) || (ImageSignature == TiffHeaderSignatureBE))
      {
        StreamImageType = ImageType.Tiff;
      }
      else if (ImageSignature == PngHeaderPart1)
      {
        StreamImageType = ImageType.Png;
      }
      else throw new ExifException(ExifErrCode.ImageTypeIsNotSupported);

      // Check stream size
      if (StreamToBeChecked.Length > int.MaxValue)
      {
        throw new ExifException(ExifErrCode.ImageHasUnsupportedFeatures);
      }

      // Check if stream is seekable
      const long CheckOffset = -2;
      long i = StreamToBeChecked.Position;
      StreamToBeChecked.Position += CheckOffset;
      if (StreamToBeChecked.Position != (i + CheckOffset))
      {
        // The stream for reading the image data must support the property "Position"
        throw new ExifException(ExifErrCode.ImageHasUnsupportedFeatures);
      }
      StreamToBeChecked.Position = i;
      return (StreamImageType);
    }


    private void EvaluateTiffHeader(byte[] TiffHeader, int TiffHeaderBytesRead, out int IfdPrimaryDataOffset)
    {
      if (TiffHeaderBytesRead < TiffHeaderLen)
      {
        throw new ExifException(ErrCodeForIllegalExifBlock);
      }

      uint TiffHeaderSignature = ReadUInt32BE(TiffHeader, 0);
      if (TiffHeaderSignature == TiffHeaderSignatureLE)
      {
        _ByteOrder = ExifByteOrder.LittleEndian;
      }
      else if (TiffHeaderSignature == TiffHeaderSignatureBE)
      {
        _ByteOrder = ExifByteOrder.BigEndian;
      }
      else throw new ExifException(ErrCodeForIllegalExifBlock);

      IfdPrimaryDataOffset = (int)ExifReadUInt32(TiffHeader, 4);
      if (IfdPrimaryDataOffset < TiffHeaderLen)
      {
        throw new ExifException(ErrCodeForIllegalExifBlock);
      }
    }


    private void SaveJpeg(Stream SourceStream, Stream DestStream)
    {
      const long FirstBlockStartPosition = 2;
      ushort BlockMarker;
      byte[] BlockContent = new byte[65536];
      byte[] TempBuffer = new byte[4];

      SourceStream.Position = FirstBlockStartPosition;
      WriteUInt16BE(TempBuffer, 0, JpegSoiMarker);
      DestStream.Write(TempBuffer, 0, 2);

      // Copy all APP0 blocks of the source file. These blocks should be the JFIF blocks.
      do
      {
        ReadJpegBlockMarker(SourceStream, out BlockMarker, out int BlockContentSize);
        if (BlockMarker == JpegSosMarker)
        {
          break; // Start of JPEG image matrix reached
        }
        long NextBlockStartPosition = SourceStream.Position + BlockContentSize;
        if (BlockMarker == JpegApp0Marker)
        {
          WriteUInt16BE(TempBuffer, 0, BlockMarker);
          WriteUInt16BE(TempBuffer, 2, (ushort)(BlockContentSize + 2));
          DestStream.Write(TempBuffer, 0, 4);
          if (SourceStream.Read(BlockContent, 0, BlockContentSize) == BlockContentSize)
          {
            DestStream.Write(BlockContent, 0, BlockContentSize);
          }
          else throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
        }
        SourceStream.Position = NextBlockStartPosition;
      } while (true);

      // Write new EXIF block if the new EXIF block is not empty
      CreateExifBlock(out FlexArray NewExifBlock, TiffHeaderLen);
      int NewExifBlockLen = NewExifBlock.Length;
      if (NewExifBlockLen > JpegMaxExifBlockLen)
      {
        throw new ExifException(ExifErrCode.ExifDataAreTooLarge);
      }
      else if (NewExifBlockLen > 0)
      {
        WriteUInt16BE(TempBuffer, 0, JpegApp1Marker);
        WriteUInt16BE(TempBuffer, 2, (ushort)(2 + 6 + TiffHeaderLen + NewExifBlockLen));
        DestStream.Write(TempBuffer, 0, 4);
        DestStream.Write(JpegExifSignature, 0, JpegExifSignature.Length);
        CreateTiffHeader(out byte[] TiffHeader, TiffHeaderLen);
        DestStream.Write(TiffHeader, 0, TiffHeader.Length);
        DestStream.Write(NewExifBlock.Buffer, 0, NewExifBlockLen);
      }

      // Return to the first block of the image
      SourceStream.Position = FirstBlockStartPosition;
      do
      {
        ReadJpegBlock(SourceStream, BlockContent, out int BlockContentSize, out BlockMarker, out ImageFileBlock BlockType, out _);
        if (BlockMarker == JpegSosMarker)
        {
          // Start of JPEG image matrix reached. Copy all remaining data from source stream to destination stream
          // without further interpretation.
          WriteUInt16BE(TempBuffer, 0, BlockMarker);
          DestStream.Write(TempBuffer, 0, 2);
          int k;
          do
          {
            k = SourceStream.Read(BlockContent, 0, BlockContent.Length);
            DestStream.Write(BlockContent, 0, k);
          } while (k == BlockContent.Length);
          break; // Destination stream was completely written
        }

        bool CopyBlockFromSourceStream = true;
        if (BlockMarker == JpegApp0Marker)
        {
          CopyBlockFromSourceStream = false; // All APP0 blocks have already been copied
        }
        else if (BlockType == ImageFileBlock.Exif)
        {
          CopyBlockFromSourceStream = false; // EXIF block has already been written
        }
        else if ((BlockType == ImageFileBlock.Xmp) && (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Removed))
        {
          CopyBlockFromSourceStream = false;
        }
        else if ((BlockType == ImageFileBlock.Iptc) && (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Removed))
        {
          CopyBlockFromSourceStream = false;
        }
        else if ((BlockType == ImageFileBlock.JpegComment) && (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Removed))
        {
          CopyBlockFromSourceStream = false;
        }

        if (CopyBlockFromSourceStream)
        {
          WriteUInt16BE(TempBuffer, 0, BlockMarker);
          WriteUInt16BE(TempBuffer, 2, (ushort)(BlockContentSize + 2));
          DestStream.Write(TempBuffer, 0, 4);
          DestStream.Write(BlockContent, 0, BlockContentSize);
        }
      } while (true);
    }


    private void SaveTiff(Stream SourceStream, Stream DestStream)
    {
      int k, ImageNumber = 0, NextExifBlockPointerIndex = 0;
      byte[] TempBuffer = new byte[65536];
      ExifData CurrentImageExif;
      FlexArray ExifBlockAsBinaryData = null;

      SourceStream.Position = 0;
      byte[] TiffHeader = new byte[TiffHeaderLen];
      if (SourceStream.Read(TiffHeader, 0, TiffHeaderLen) != TiffHeaderLen)
      {
        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
      }
      ExifByteOrder SourceStreamByteOrder = ExifByteOrder.LittleEndian;
      uint TiffHeaderSignature = ReadUInt32BE(TiffHeader, 0);
      if (TiffHeaderSignature == TiffHeaderSignatureBE)
      {
        SourceStreamByteOrder = ExifByteOrder.BigEndian;
      }
      if (_ByteOrder != SourceStreamByteOrder)
      {
        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
      }
      int SourceStreamExifBlockOffset = (int)ExifReadUInt32(TiffHeader, 4);
      uint CurrentDestStreamOffset = TiffHeaderLen;

      // TIFF file structure when saving a multi-page TIFF file:
      //
      // --------------------------------------------
      // TIFF Header (8 bytes)
      // --------------------------------------------
      // Image 1 matrix
      // --------------------------------------------
      // Image 1 EXIF block
      // --------------------------------------------
      // Image 2 matrix
      // --------------------------------------------
      // Image 2 EXIF block
      // --------------------------------------------
      // ...

      do
      {
        CurrentImageExif = CreateSubExifData(SourceStream, ref SourceStreamExifBlockOffset, SourceStreamByteOrder,
          out uint[] SegmentOffsetTable, out uint[] SegmentByteCountTable,
          out ExifTag SegmentOffsetsTag, out ExifTag SegmentByteCountsTag);

        if (ImageNumber == 0)
        {
          CurrentImageExif = this; // Discard the EXIF block returned by "CreateSubExifData"
        }

        if (!CurrentImageExif.TagExists(ExifTag.ImageWidth) || !CurrentImageExif.TagExists(ExifTag.ImageLength))
        {
          throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
        }
        CurrentImageExif.RemoveTag(ExifTag.FreeOffsets); // Remove tags for areas of unused bytes
        CurrentImageExif.RemoveTag(ExifTag.FreeByteCounts);

        // Assign new segment offsets.
        int ImageSegmentCount = SegmentOffsetTable.Length;
        CurrentImageExif.SetTagValueCount(SegmentOffsetsTag, ImageSegmentCount, ExifTagType.ULong);
        CurrentImageExif.SetTagValueCount(SegmentByteCountsTag, ImageSegmentCount, ExifTagType.ULong);
        for (k = 0; k < ImageSegmentCount; k++)
        {
          CurrentImageExif.SetTagValue(SegmentOffsetsTag, CurrentDestStreamOffset, ExifTagType.ULong, k);
          uint SegmentSize = SegmentByteCountTable[k];
          CurrentImageExif.SetTagValue(SegmentByteCountsTag, SegmentSize, ExifTagType.ULong, k);
          CurrentDestStreamOffset += SegmentSize;
        }
        bool HasImageMatrixFillByte = false;
        if ((CurrentDestStreamOffset & 0x1) != 0)
        {
          CurrentDestStreamOffset++;
          HasImageMatrixFillByte = true;
        }
        if (CurrentDestStreamOffset > int.MaxValue) throw new ExifException(ExifErrCode.ExifDataAreTooLarge);

        if (ImageNumber == 0)
        {
          // Write TIFF header to destination stream
          CurrentImageExif.ExifWriteUInt32(TiffHeader, 4, CurrentDestStreamOffset);
          DestStream.Write(TiffHeader, 0, TiffHeaderLen);
        }
        else
        {
          // Insert the the pointer to the EXIF block of the current image into the binary data of the EXIF block of
          // the previous image. Then write the previous EXIF block to destination stream.
          CurrentImageExif.ExifWriteUInt32(ExifBlockAsBinaryData.Buffer, NextExifBlockPointerIndex, CurrentDestStreamOffset);
          DestStream.Write(ExifBlockAsBinaryData.Buffer, 0, ExifBlockAsBinaryData.Length);
        }

        // Write image matrix: copy image segments from source to destination stream
        for (k = 0; k < ImageSegmentCount; k++)
        {
          int RemainingByteCount = (int)SegmentByteCountTable[k];
          int BytesToRead = TempBuffer.Length;
          SourceStream.Position = SegmentOffsetTable[k];
          do
          {
            if (BytesToRead > RemainingByteCount) BytesToRead = RemainingByteCount;
            if (SourceStream.Read(TempBuffer, 0, BytesToRead) == BytesToRead)
            {
              DestStream.Write(TempBuffer, 0, BytesToRead);
              RemainingByteCount -= BytesToRead;
            }
            else throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
          } while (RemainingByteCount > 0);
        }
        if (HasImageMatrixFillByte) DestStream.WriteByte(0);

        NextExifBlockPointerIndex = CurrentImageExif.CreateExifBlock(out ExifBlockAsBinaryData, (int)CurrentDestStreamOffset);
        CurrentDestStreamOffset += (uint)ExifBlockAsBinaryData.Length;
        if (CurrentDestStreamOffset > int.MaxValue) throw new ExifException(ExifErrCode.ExifDataAreTooLarge);

        ImageNumber++;
      } while (SourceStreamExifBlockOffset != 0);

      DestStream.Write(ExifBlockAsBinaryData.Buffer, 0, ExifBlockAsBinaryData.Length);
    }


    private static ExifData CreateSubExifData(Stream TiffStream, ref int ExifBlockOffset, ExifByteOrder ByteOrder, out uint[] SegmentOffsetTable,
      out uint[] SegmentByteCountTable, out ExifTag SegmentOffsetsTag, out ExifTag SegmentByteCountsTag)
    {
      ExifData SubExifBlock = new ExifData();
      SubExifBlock._ByteOrder = ByteOrder;
      SubExifBlock.ImageType = ImageType.Tiff;
      SubExifBlock.SourceExifBlock = null;
      SubExifBlock.SourceExifStream = TiffStream;

      ExifBlockOffset = SubExifBlock.EvaluateExifBlock(ExifBlockOffset);
            if (SubExifBlock.GetTagItem(ExifTag.StripOffsets, out TagItem SegmentOffsetsTagItem) &&
                SubExifBlock.GetTagItem(ExifTag.StripByteCounts, out TagItem SegmentByteCountsTagItem))
            {
                SegmentOffsetsTag = ExifTag.StripOffsets;
                SegmentByteCountsTag = ExifTag.StripByteCounts;
            }
            else if (SubExifBlock.GetTagItem(ExifTag.TileOffsets, out SegmentOffsetsTagItem) &&
                     SubExifBlock.GetTagItem(ExifTag.TileByteCounts, out SegmentByteCountsTagItem))
            {
                SegmentOffsetsTag = ExifTag.TileOffsets;
                SegmentByteCountsTag = ExifTag.TileByteCounts;
            }
            else throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

            int ValueCount = SegmentOffsetsTagItem.ValueCount;
      SegmentOffsetTable = new uint[ValueCount];
      SegmentByteCountTable = new uint[ValueCount];
      bool IsOffsetTable16Bit = (SegmentOffsetsTagItem.TagType == ExifTagType.UShort);
      bool ByteCountTable16Bit = (SegmentByteCountsTagItem.TagType == ExifTagType.UShort);
      uint v;
      for (int i = 0; i < ValueCount; i++)
      {
        if (IsOffsetTable16Bit)
        {
          v = SubExifBlock.ExifReadUInt16(SegmentOffsetsTagItem.ValueData, SegmentOffsetsTagItem.ValueIndex + (i << 1));
        }
        else v = SubExifBlock.ExifReadUInt32(SegmentOffsetsTagItem.ValueData, SegmentOffsetsTagItem.ValueIndex + (i << 2));
        SegmentOffsetTable[i] = v;
        if (ByteCountTable16Bit)
        {
          v = SubExifBlock.ExifReadUInt16(SegmentByteCountsTagItem.ValueData, SegmentByteCountsTagItem.ValueIndex + (i << 1));
        }
        else v = SubExifBlock.ExifReadUInt32(SegmentByteCountsTagItem.ValueData, SegmentByteCountsTagItem.ValueIndex + (i << 2));
        SegmentByteCountTable[i] = v;
      }
      return (SubExifBlock);
    }


    private void SavePng(Stream SourceStream, Stream DestStream)
    {
      uint ChunkType;
      byte[] TempData = new byte[65536];
      byte[] BlockContent = new byte[30]; // Used for signature data at the beginning of the block

      SourceStream.Position = 8;
      WriteUInt32BE(TempData, 0, PngHeaderPart1);
      WriteUInt32BE(TempData, 4, PngHeaderPart2);
      DestStream.Write(TempData, 0, 8);
      do
      {
        bool CopyBlockFromSourceStream = true;
        long BlockStartStreamPos = SourceStream.Position;
        ReadPngBlockHeader(SourceStream, TempData, out int DataLength, out ChunkType);
        DetectPngImageBlock(SourceStream, BlockContent, ChunkType, out ImageFileBlock BlockType);
        if (BlockType != ImageFileBlock.Unknown)
        {
          if ((BlockType == ImageFileBlock.Exif) || (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Removed))
          {
            CopyBlockFromSourceStream = false; // EXIF block will always be replaced by a new EXIF block
          }
        }

        if (CopyBlockFromSourceStream)
        {
          // Copy current block from source to destination stream
          SourceStream.Position = BlockStartStreamPos;
          int RemainingByteCount = DataLength + 12;
          int BytesToRead = TempData.Length;
          do
          {
            if (BytesToRead > RemainingByteCount) BytesToRead = RemainingByteCount;
            if (SourceStream.Read(TempData, 0, BytesToRead) == BytesToRead)
            {
              DestStream.Write(TempData, 0, BytesToRead);
              RemainingByteCount -= BytesToRead;
            }
            else throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
          } while (RemainingByteCount > 0);
        }
        else
        {
          SourceStream.Position = BlockStartStreamPos + DataLength + 12;
        }

        if (ChunkType == PngIhdrChunk)
        {
          // Write new EXIF block after the IHDR chunk if the new EXIF block is not empty
          CreateExifBlock(out FlexArray NewExifBlock, TiffHeaderLen);
          int NewExifBlockLen = NewExifBlock.Length;
          if (NewExifBlockLen > 0)
          {
            CreateTiffHeader(out byte[] TiffHeader, TiffHeaderLen);
            uint BlockLength = TiffHeaderLen + (uint)NewExifBlockLen;
            WriteUInt32BE(TempData, 0, BlockLength);
            WriteUInt32BE(TempData, 4, PngExifChunk);
            DestStream.Write(TempData, 0, 8);
            DestStream.Write(TiffHeader, 0, TiffHeaderLen);
            DestStream.Write(NewExifBlock.Buffer, 0, NewExifBlockLen);

            uint Crc32 = CalculateCrc32(TempData, 4, 4, false); // Checksum over chunk type
            Crc32 = CalculateCrc32(TiffHeader, 0, TiffHeaderLen, false, Crc32);
            Crc32 = CalculateCrc32(NewExifBlock.Buffer, 0, NewExifBlockLen, true, Crc32);
            WriteUInt32BE(TempData, 0, Crc32);
            DestStream.Write(TempData, 0, 4);
          }
        }

      } while (ChunkType != PngIendChunk);
    }


    private void SetEmptyExifBlock()
    {
      // The TIFF header, which is stored in the array elements from 0 to 7, will not be initialized because it is not used any more.
      SourceExifBlock = new byte[TiffHeaderLen + MinExifBlockLen];
      ExifWriteUInt16(SourceExifBlock, TiffHeaderLen, 0); // Number of tags of IFD Primary Data
      ExifWriteUInt32(SourceExifBlock, TiffHeaderLen + 2, 0); // Offset of IFD Thumbnail Data
      EvaluateExifBlock(TiffHeaderLen);
    }


    private void WriteTagToFlexArray(TagItem TempTagData, FlexArray WriteData, ref int TagDataIndex, int ExifBlockOffset)
    {
      int i, ByteCount;
      byte v;

      i = TagDataIndex;
      TagDataIndex += 12;
      ExifWriteUInt16(WriteData.Buffer, i, (ushort)TempTagData.TagId);
      ExifWriteUInt16(WriteData.Buffer, i + 2, (ushort)TempTagData.TagType);
      ExifWriteUInt32(WriteData.Buffer, i + 4, (uint)TempTagData.ValueCount);
      ByteCount = GetTagByteCount(TempTagData.TagType, TempTagData.ValueCount);
      if (ByteCount <= 4)
      {
        // The tag does not have outsourced data. In this case exactly 4 bytes have to be written.
        WriteData.Buffer[i + 8] = TempTagData.ValueData[TempTagData.ValueIndex];
        v = 0;
        if (ByteCount >= 2) v = TempTagData.ValueData[TempTagData.ValueIndex + 1];
        WriteData.Buffer[i + 9] = v;
        v = 0;
        if (ByteCount >= 3) v = TempTagData.ValueData[TempTagData.ValueIndex + 2];
        WriteData.Buffer[i + 10] = v;
        v = 0;
        if (ByteCount >= 4) v = TempTagData.ValueData[TempTagData.ValueIndex + 3];
        WriteData.Buffer[i + 11] = v;
      }
      else
      {
        // The tag has outsourced data. The outsourced data is added at the end of array "WriteData".
        int OutsourcedDataIndex = WriteData.Length;
        ExifWriteUInt32(WriteData.Buffer, i + 8, (uint)(ExifBlockOffset + OutsourcedDataIndex));
        WriteData.Length += ByteCount; // Allocate memory in array "WriteData"
        Array.Copy(TempTagData.ValueData, TempTagData.ValueIndex, WriteData.Buffer, OutsourcedDataIndex, ByteCount);
        if ((ByteCount & 0x1) != 0)
        {
          // The EXIF block must have a 16 bit alignment. "ByteCount" is odd, therefore add a fill byte.
          WriteData.Length++; // Allocate 1 byte in array "WriteData"
          WriteData.Buffer[WriteData.Length - 1] = 0;
        }
      }
    }


    private int CreateExifBlock(out FlexArray NewExifBlock, int CurrentExifBlockOffset)
    {
      int TiffNextExifBlockPointerIndex = 0;

      FlexArray WriteExifBlock = new FlexArray(JpegMaxExifBlockLen);
      NewExifBlock = WriteExifBlock;
      int WriteIndex = 0;
      UpdateIfdPointerTags(out TagItem PrivateDataPointerTag, out TagItem GpsInfoDataPointerTag, out TagItem InteroperabilityPointerTag);
      CreateIfdPrimaryData(WriteExifBlock, ref WriteIndex, CurrentExifBlockOffset, out int PrivateDataIfdPointerIndex,
        out int GpsInfoDataIfdPointerIndex, out int ThumbnailDataIfdPointerIndex);
      CreateIfdPrivateData(WriteExifBlock, ref WriteIndex, CurrentExifBlockOffset, PrivateDataIfdPointerIndex, PrivateDataPointerTag,
        out int InteroperabilityIfdPointerIndex);
      CreateIfdGpsInfoData(WriteExifBlock, ref WriteIndex, CurrentExifBlockOffset, GpsInfoDataIfdPointerIndex, GpsInfoDataPointerTag);
      CreateIfdInteroperability(WriteExifBlock, ref WriteIndex, CurrentExifBlockOffset, InteroperabilityIfdPointerIndex, InteroperabilityPointerTag);
      if (ImageType == ImageType.Tiff)
      {
        // TIFF files don't have a thumbnail image, but they may have multiple images
        TiffNextExifBlockPointerIndex = ThumbnailDataIfdPointerIndex;
        ExifWriteUInt32(WriteExifBlock.Buffer, TiffNextExifBlockPointerIndex, 0); // Set pointer to next image provisorily to 0
      }
      else CreateIfdThumbnailData(WriteExifBlock, ref WriteIndex, CurrentExifBlockOffset, ThumbnailDataIfdPointerIndex);

      // Check if new EXIF block is empty. Empty EXIF blocks should not be written instead the EXIF block should be removed.
      if (WriteExifBlock.Length <= MinExifBlockLen)
      {
        WriteExifBlock.Length = 0;
      }

      return (TiffNextExifBlockPointerIndex);
    }


    private void CreateTiffHeader(out byte[] TiffHeader, int ExifBlockOffset)
    {
      TiffHeader = new byte[TiffHeaderLen];
      if (ByteOrder == ExifByteOrder.BigEndian)
      {
        WriteUInt32BE(TiffHeader, 0, TiffHeaderSignatureBE);
      }
      else if (ByteOrder == ExifByteOrder.LittleEndian)
      {
        WriteUInt32BE(TiffHeader, 0, TiffHeaderSignatureLE);
      }
      else throw new ExifException(ExifErrCode.InternalError);
      ExifWriteUInt32(TiffHeader, 4, (uint)ExifBlockOffset);
    }


    private void UpdateIfdPointerTags(out TagItem PrivateDataPointerTag, out TagItem GpsInfoDataPointerTag, out TagItem InteroperabilityPointerTag)
    {
      Dictionary<ExifTagId, TagItem> PrivateDataIfdTable = TagTable[(uint)ExifIfd.PrivateData];
      Dictionary<ExifTagId, TagItem> InteroperabilityIfdTable = TagTable[(uint)ExifIfd.Interoperability];
      PrivateDataIfdTable.TryGetValue(ExifTagId.InteroperabilityIfdPointer, out InteroperabilityPointerTag);
      int InteroperabilityTagCount = InteroperabilityIfdTable.Count;
      if (InteroperabilityTagCount > 0)
      {
        if (InteroperabilityPointerTag == null)
        {
          InteroperabilityPointerTag = new TagItem(ExifTag.InteroperabilityIfdPointer, ExifTagType.ULong, 1);
          PrivateDataIfdTable.Add(ExifTagId.InteroperabilityIfdPointer, InteroperabilityPointerTag);
        }
      }
      else
      {
        if (InteroperabilityPointerTag != null)
        {
          PrivateDataIfdTable.Remove(ExifTagId.InteroperabilityIfdPointer);
        }
      }

      Dictionary<ExifTagId, TagItem> PrimaryDataIfdTable = TagTable[(uint)ExifIfd.PrimaryData];
      Dictionary<ExifTagId, TagItem> GpsInfoDataIfdTable = TagTable[(uint)ExifIfd.GpsInfoData];
      PrimaryDataIfdTable.TryGetValue(ExifTagId.GpsInfoIfdPointer, out GpsInfoDataPointerTag);
      int GpsInfoDataTagCount = GpsInfoDataIfdTable.Count;
      if (GpsInfoDataTagCount > 0)
      {
        if (GpsInfoDataPointerTag == null)
        {
          GpsInfoDataPointerTag = new TagItem(ExifTag.GpsInfoIfdPointer, ExifTagType.ULong, 1);
          PrimaryDataIfdTable.Add(ExifTagId.GpsInfoIfdPointer, GpsInfoDataPointerTag);
        }
      }
      else
      {
        if (GpsInfoDataPointerTag != null)
        {
          PrimaryDataIfdTable.Remove(ExifTagId.GpsInfoIfdPointer);
        }
      }

      PrimaryDataIfdTable.TryGetValue(ExifTagId.ExifIfdPointer, out PrivateDataPointerTag);
      if (PrivateDataIfdTable.Count > 0)
      {
        if (PrivateDataPointerTag == null)
        {
          PrivateDataPointerTag = new TagItem(ExifTag.ExifIfdPointer, ExifTagType.ULong, 1);
          PrimaryDataIfdTable.Add(ExifTagId.ExifIfdPointer, PrivateDataPointerTag);
        }
      }
      else
      {
        if (PrivateDataPointerTag != null)
        {
          PrimaryDataIfdTable.Remove(ExifTagId.ExifIfdPointer);
        }
      }
    }


    private void CreateIfdPrimaryData(FlexArray WriteExifBlock, ref int WriteIndex, int ExifBlockOffset, out int PrivateDataIfdPointerIndex,
      out int GpsInfoDataIfdPointerIndex, out int ThumbnailDataIfdPointerIndex)
    {
      Dictionary<ExifTagId, TagItem> PrimaryDataIfdTable = TagTable[(uint)ExifIfd.PrimaryData];
      int PrimaryDataTagCount = PrimaryDataIfdTable.Count;
      PrivateDataIfdPointerIndex = -1;
      GpsInfoDataIfdPointerIndex = -1;
      int IfdFixedSize = 2 + PrimaryDataTagCount * 12 + 4; // "+ 4" for thumbnail data pointer
      WriteExifBlock.Length += IfdFixedSize; // Allocate memory in array "WriteExifBlock"
      ExifWriteUInt16(WriteExifBlock.Buffer, WriteIndex, (ushort)PrimaryDataTagCount);
      WriteIndex += 2;
      foreach (TagItem t in PrimaryDataIfdTable.Values)
      {
        if (t.TagId == ExifTagId.ExifIfdPointer)
        {
          t.TagType = ExifTagType.ULong;
          PrivateDataIfdPointerIndex = WriteIndex + 8;  // The "Private Data IFD" pointer is unknown at this time and a dummy
                                                        // value is written for it. The index is stored, at which the actual pointer can be written later.
        }
        else if (t.TagId == ExifTagId.GpsInfoIfdPointer)
        {
          t.TagType = ExifTagType.ULong;
          GpsInfoDataIfdPointerIndex = WriteIndex + 8; // The "GPS Info Data IFD" pointer is unknown at this time and a dummy
                                                       // value is written for it. The index is stored, at which the actual pointer can be written later.
        }
        WriteTagToFlexArray(t, WriteExifBlock, ref WriteIndex, ExifBlockOffset);
      }

      // Now the thumbnail data pointer should be written. Because it is unknown at this time the index is stored.
      ThumbnailDataIfdPointerIndex = WriteIndex;
      WriteIndex = WriteExifBlock.Length; // Write the next IFD behind the outsourced data of the IFD Primary Data
    }


    private void CreateIfdPrivateData(FlexArray WriteExifBlock, ref int WriteIndex, int ExifBlockOffset, int PrivateDataIfdPointerIndex, 
      TagItem PrivateDataPointerTag, out int InteroperabilityIfdPointerIndex)
    {
      Dictionary<ExifTagId, TagItem> PrivateDataIfdTable = TagTable[(uint)ExifIfd.PrivateData];
      int PrivateDataTagCount = PrivateDataIfdTable.Count;
      InteroperabilityIfdPointerIndex = -1;
      if (PrivateDataTagCount > 0)
      {
        int iPrivateDataStart = WriteIndex;
        int MakerNoteDataPointerIndex, OffsetSchemaValueIndex;
        TagItem OffsetSchemaTag;
        bool AddOffsetSchemaTag;
        ExifWriteUInt32(PrivateDataPointerTag.ValueData, PrivateDataPointerTag.ValueIndex, (uint)(ExifBlockOffset + WriteIndex));
        ExifWriteUInt32(WriteExifBlock.Buffer, PrivateDataIfdPointerIndex, (uint)(ExifBlockOffset + WriteIndex));
        do
        {
          MakerNoteDataPointerIndex = 0;
          OffsetSchemaValueIndex = 0;
          OffsetSchemaTag = null;
          int IfdFixedSize = 2 + PrivateDataTagCount * 12 + 4; // "+ 4" for null pointer at the end
          WriteExifBlock.Length += IfdFixedSize; // Allocate memory in array "WriteExifBlock"
          ExifWriteUInt16(WriteExifBlock.Buffer, WriteIndex, (ushort)PrivateDataTagCount);
          WriteIndex += 2;
          foreach (TagItem t in PrivateDataIfdTable.Values)
          {
            if (t.TagId == ExifTagId.InteroperabilityIfdPointer)
            {
              t.TagType = ExifTagType.ULong;
              InteroperabilityIfdPointerIndex = WriteIndex + 8; // The "Interoperability IFD" pointer is unknown at this time and a dummy
                                                                // value is written for it. The index is stored, at which the actual pointer can be written later.
            }
            else if (t.TagId == ExifTagId.MakerNote)
            {
              MakerNoteDataPointerIndex = WriteIndex + 8;
            }
            else if (t.TagId == ExifTagId.OffsetSchema)
            {
              t.TagType = ExifTagType.SLong;
              OffsetSchemaValueIndex = WriteIndex + 8;
              OffsetSchemaTag = t;
            }
            WriteTagToFlexArray(t, WriteExifBlock, ref WriteIndex, ExifBlockOffset);
          }
          ExifWriteUInt32(WriteExifBlock.Buffer, WriteIndex, 0); // Write null pointer at the end of the tag list
          WriteIndex = WriteExifBlock.Length; // Write the next IFD behind the outsourced data of the IFD Private Data
          AddOffsetSchemaTag = CheckIfMakerNoteTagHasMoved(WriteExifBlock.Buffer, MakerNoteDataPointerIndex, OffsetSchemaValueIndex, OffsetSchemaTag);
          if (AddOffsetSchemaTag)
          {
            TagItem t = new TagItem(ExifTag.OffsetSchema, ExifTagType.SLong, 1);
            PrivateDataIfdTable.Add(ExifTagId.OffsetSchema, t);
            PrivateDataTagCount++;
            WriteExifBlock.Length = iPrivateDataStart; // Free all memory that was allocated for IFD Private Data
            WriteIndex = iPrivateDataStart;
          }
        } while (AddOffsetSchemaTag);
      }
    }


    private void CreateIfdGpsInfoData(FlexArray WriteExifBlock, ref int WriteIndex, int ExifBlockOffset, int GpsInfoDataIfdPointerIndex, TagItem GpsInfoDataPointerTag)
    {
      Dictionary<ExifTagId, TagItem> GpsInfoDataIfdTable = TagTable[(uint)ExifIfd.GpsInfoData];
      int GpsInfoDataTagCount = GpsInfoDataIfdTable.Count;
      if (GpsInfoDataTagCount > 0)
      {
        uint GpsInfoDataOffset = (uint)(ExifBlockOffset + WriteIndex);
        ExifWriteUInt32(GpsInfoDataPointerTag.ValueData, GpsInfoDataPointerTag.ValueIndex, GpsInfoDataOffset);
        ExifWriteUInt32(WriteExifBlock.Buffer, GpsInfoDataIfdPointerIndex, GpsInfoDataOffset);
        int IfdFixedSize = 2 + GpsInfoDataTagCount * 12 + 4;
        WriteExifBlock.Length += IfdFixedSize;
        ExifWriteUInt16(WriteExifBlock.Buffer, WriteIndex, (ushort)GpsInfoDataTagCount);
        WriteIndex += 2;
        foreach (TagItem t in GpsInfoDataIfdTable.Values)
        {
          WriteTagToFlexArray(t, WriteExifBlock, ref WriteIndex, ExifBlockOffset);
        }
        ExifWriteUInt32(WriteExifBlock.Buffer, WriteIndex, 0); // Write null pointer at the end of the tag list
        WriteIndex = WriteExifBlock.Length; // Write the next IFD behind the outsourced data of the IFD GPS Info Data
      }
    }


    private void CreateIfdInteroperability(FlexArray WriteExifBlock, ref int WriteIndex, int ExifBlockOffset, int InteroperabilityIfdPointerIndex,
      TagItem InteroperabilityPointerTag)
    {
      Dictionary<ExifTagId, TagItem> InteroperabilityIfdTable = TagTable[(uint)ExifIfd.Interoperability];
      int InteroperabilityTagCount = InteroperabilityIfdTable.Count;
      if (InteroperabilityTagCount > 0)
      {
        uint InteroperabilityOffset = (uint)(ExifBlockOffset + WriteIndex);
        ExifWriteUInt32(InteroperabilityPointerTag.ValueData, InteroperabilityPointerTag.ValueIndex, InteroperabilityOffset);
        ExifWriteUInt32(WriteExifBlock.Buffer, InteroperabilityIfdPointerIndex, InteroperabilityOffset);
        int IfdFixedSize = 2 + InteroperabilityTagCount * 12 + 4;
        WriteExifBlock.Length += IfdFixedSize;
        ExifWriteUInt16(WriteExifBlock.Buffer, WriteIndex, (ushort)InteroperabilityTagCount);
        WriteIndex += 2;
        foreach (TagItem t in InteroperabilityIfdTable.Values)
        {
          WriteTagToFlexArray(t, WriteExifBlock, ref WriteIndex, ExifBlockOffset);
        }
        ExifWriteUInt32(WriteExifBlock.Buffer, WriteIndex, 0); // Write null pointer at the end of the tag list
        WriteIndex = WriteExifBlock.Length; // Write the next IFD behind the outsourced data of the IFD Interoperability
      }
    }


    private void CreateIfdThumbnailData(FlexArray WriteExifBlock, ref int WriteIndex, int ExifBlockOffset, int ThumbnailDataIfdPointerIndex)
    {
      Dictionary<ExifTagId, TagItem> ThumbnailDataIfdTable = TagTable[(uint)ExifIfd.ThumbnailData];
      int ThumbnailDataTagCount = ThumbnailDataIfdTable.Count;
      if ((ThumbnailDataTagCount > 0) || (ThumbnailImageExists()))
      {
        ExifWriteUInt32(WriteExifBlock.Buffer, ThumbnailDataIfdPointerIndex, (uint)(ExifBlockOffset + WriteIndex));
        int IfdFixedSize = 2 + ThumbnailDataTagCount * 12 + 4;
        WriteExifBlock.Length += IfdFixedSize;
        ExifWriteUInt16(WriteExifBlock.Buffer, WriteIndex, (ushort)ThumbnailDataTagCount);
        WriteIndex += 2;
        int ThumbnailImagePointerIndex = -1;
        TagItem ThumbnailImagePointerTag = null;
        bool ThumbnailImageSizeTagExists = false;
        foreach (TagItem t in ThumbnailDataIfdTable.Values)
        {
          if (t.TagId == ExifTagId.JpegInterchangeFormat)
          {
            // The tag "TagId_JpegInterchangeFormat" stores a pointer to the thumbnail image.
            // The pointer is not known at this time, so the index, where the pointer has to be written, is stored.
            t.TagType = ExifTagType.ULong;
            ThumbnailImagePointerTag = t;
            ThumbnailImagePointerIndex = WriteIndex + 8;
          }
          else if (t.TagId == ExifTagId.JpegInterchangeFormatLength)
          {
            t.TagType = ExifTagType.ULong;
            ExifWriteUInt32(t.ValueData, t.ValueIndex, (uint)ThumbnailByteCount);
            ThumbnailImageSizeTagExists = true;
          }
          WriteTagToFlexArray(t, WriteExifBlock, ref WriteIndex, ExifBlockOffset);
        }

        if (ThumbnailImage != null)
        {
          // A thumbnail image is defined
          if ((ThumbnailImagePointerIndex < 0) || (ThumbnailImageSizeTagExists == false))
          {
            throw new ExifException(ExifErrCode.InternalError);
          }
          else
          {
            int ThumbnailImageIndex = WriteExifBlock.Length;
            uint ThumbnailImageOffset = (uint)(ExifBlockOffset + ThumbnailImageIndex);
            ExifWriteUInt32(ThumbnailImagePointerTag.ValueData, ThumbnailImagePointerTag.ValueIndex, ThumbnailImageOffset);
            ExifWriteUInt32(WriteExifBlock.Buffer, ThumbnailImagePointerIndex, ThumbnailImageOffset);
            WriteExifBlock.Length += ThumbnailByteCount;
            Array.Copy(ThumbnailImage, ThumbnailStartIndex, WriteExifBlock.Buffer, ThumbnailImageIndex, ThumbnailByteCount);
            if ((WriteExifBlock.Length & 0x1) != 0)
            {
              WriteExifBlock.Length++;
              WriteExifBlock[WriteExifBlock.Length - 1] = 0; // Insert fill byte
            }
          }
        }
        else
        {
          // A thumbnail image is not defined
          if (ThumbnailImagePointerIndex >= 0)
          {
            throw new ExifException(ExifErrCode.InternalError);
          }
        }
        ExifWriteUInt32(WriteExifBlock.Buffer, WriteIndex, 0); // Write null pointer at the end of the tag table
      }
      else
      {
        ExifWriteUInt32(WriteExifBlock.Buffer, ThumbnailDataIfdPointerIndex, 0);
      }
    }


    private bool CheckIfMakerNoteTagHasMoved(byte[] DataBlock, int MakerNoteDataPointerIndex, int OffsetSchemaValueIndex, TagItem OffsetSchemaTag)
    {
      bool MakerNoteHasMovedAndOffsetSchemaTagRequired = false;
      int MakerNoteCurrentOffset;

      if ((MakerNoteOriginalOffset > 0) && (MakerNoteDataPointerIndex > 0))
      {
        // "MakerNote" has existed and is now existing
        MakerNoteCurrentOffset = (int)ExifReadUInt32(DataBlock, MakerNoteDataPointerIndex);
        if (MakerNoteOriginalOffset != MakerNoteCurrentOffset)
        {
          // "MakerNote" has moved
          if (OffsetSchemaValueIndex > 0)
          {
            int NewOffsetDifference = MakerNoteCurrentOffset - MakerNoteOriginalOffset;
            ExifWriteUInt32(DataBlock, OffsetSchemaValueIndex, (uint)NewOffsetDifference);
            ExifWriteUInt32(OffsetSchemaTag.ValueData, OffsetSchemaTag.ValueIndex, (uint)NewOffsetDifference);
          }
          else
          {
            MakerNoteHasMovedAndOffsetSchemaTagRequired = true;
          }
        }
        else
        {
          // "MakerNote" has not moved
          if (OffsetSchemaValueIndex > 0)
          {
            ExifWriteUInt32(DataBlock, OffsetSchemaValueIndex, 0); // Set move offset to 0
            ExifWriteUInt32(OffsetSchemaTag.ValueData, OffsetSchemaTag.ValueIndex, 0);
          }
        }
      }
      else
      {
        if (OffsetSchemaValueIndex > 0)
        {
          ExifWriteUInt32(DataBlock, OffsetSchemaValueIndex, 0); // Set move offset to 0
          ExifWriteUInt32(OffsetSchemaTag.ValueData, OffsetSchemaTag.ValueIndex, 0);
        }
      }
      return (MakerNoteHasMovedAndOffsetSchemaTagRequired);
    }


    // Create a tag item and initialize it with the data, which are stored in "IfdRawData[IfdRawDataIndex]". 
    // The data values of the tag are not copied, instead a reference to the array "IfdRawData" is assigned.
    private TagItem CreateTagWithReferenceToIfdRawData(byte[] IfdRawData, int IfdRawDataIndex)
    {
      int ValueIndex, ValueByteCount, AllocatedByteCount, OriginalOffset;
      byte[] TagData;

      ExifTagId TagId = (ExifTagId)ExifReadUInt16(IfdRawData, IfdRawDataIndex);
      ExifTagType TagType = (ExifTagType)ExifReadUInt16(IfdRawData, IfdRawDataIndex + 2);
      uint ValueCount = ExifReadUInt32(IfdRawData, IfdRawDataIndex + 4);

      if (ValueCount > MaxTagValueCount) throw new ExifException(ErrCodeForIllegalExifBlock);
      ValueByteCount = GetTagByteCount(TagType, (int)ValueCount);
      if (ValueByteCount <= 4)
      {
        // The tag doesn't have outsourced data. In this case a memory space with a size of 4 bytes is available for the data.
        ValueIndex = IfdRawDataIndex + 8;
        TagData = IfdRawData;
        AllocatedByteCount = 4;
        OriginalOffset = 0;
      }
      else
      {
        // The tag has outsourced data.
        OriginalOffset = (int)ExifReadUInt32(IfdRawData, IfdRawDataIndex + 8);
        AllocatedByteCount = ValueByteCount;
        GetOutsourcedData(OriginalOffset, AllocatedByteCount, out TagData, out ValueIndex);
      }
      TagItem TempTagItem = new TagItem(TagId, TagType, (int)ValueCount, TagData, ValueIndex, AllocatedByteCount, OriginalOffset);
      return (TempTagItem);
    }


    private void GetOutsourcedData(int DataOffset, int DataLen, out byte[] TagData, out int TagDataIndex)
    {
      if (SourceExifBlock != null)
      {
        if ((uint)(DataOffset + DataLen) <= (uint)SourceExifBlock.Length)
        {
          TagData = SourceExifBlock;
          TagDataIndex = DataOffset;
        }
        else throw new ExifException(ErrCodeForIllegalExifBlock);
      }
      else
      {
        TagData = new byte[DataLen];
        SourceExifStream.Position = DataOffset; // + ExifBlockFilePosition;
        if (SourceExifStream.Read(TagData, 0, DataLen) != DataLen)
        {
          throw new ExifException(ErrCodeForIllegalExifBlock);
        }
        TagDataIndex = 0;
      }
    }


    private void InitIfdPrimaryData(byte[] IfdRawData, int IfdRawDataIndex, out int PrivateDataOffset, out int GpsInfoDataOffset, out int NextImageOffset)
    {
      PrivateDataOffset = 0;
      GpsInfoDataOffset = 0;
      int TagCount = ExifReadUInt16(IfdRawData, IfdRawDataIndex);
      IfdRawDataIndex += 2;
      var PrimaryDataIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
      TagTable[(uint)ExifIfd.PrimaryData] = PrimaryDataIfdTable;
      int j = 0;
      while (j < TagCount)
      {
        TagItem TempTagData = CreateTagWithReferenceToIfdRawData(IfdRawData, IfdRawDataIndex);
        if (AddIfNotExists(PrimaryDataIfdTable, TempTagData))
        {
          if (TempTagData.TagId == ExifTagId.ExifIfdPointer)
          {
            PrivateDataOffset = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
          }
          else if (TempTagData.TagId == ExifTagId.GpsInfoIfdPointer)
          {
            GpsInfoDataOffset = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
          }
        }
        IfdRawDataIndex += 12;
        j++;
      }
      NextImageOffset = (int)ExifReadUInt32(IfdRawData, IfdRawDataIndex);
    }


    private void InitIfdPrivateData(byte[] IfdRawData, int IfdRawDataIndex, out int InteroperabilityOffset)
    {
      int MakerNoteOffset, OffsetSchemaValue;

      InteroperabilityOffset = 0;
      MakerNoteOffset = 0;
      OffsetSchemaValue = 0;
      int TagCount = ExifReadUInt16(IfdRawData, IfdRawDataIndex);
      IfdRawDataIndex += 2;
      var PrivateDataIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
      TagTable[(uint)ExifIfd.PrivateData] = PrivateDataIfdTable;
      int j = 0;
      while (j < TagCount)
      {
        TagItem TempTagData = CreateTagWithReferenceToIfdRawData(IfdRawData, IfdRawDataIndex);
        if (AddIfNotExists(PrivateDataIfdTable, TempTagData))
        {
          if (TempTagData.TagId == ExifTagId.InteroperabilityIfdPointer)
          {
            InteroperabilityOffset = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
          }
          else if (TempTagData.TagId == ExifTagId.MakerNote)
          {
            MakerNoteOffset = TempTagData.OriginalDataOffset;
          }
          else if (TempTagData.TagId == ExifTagId.OffsetSchema)
          {
            OffsetSchemaValue = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
          }
        }
        IfdRawDataIndex += 12;
        j++;
      }

      if (MakerNoteOffset > 0)
      {
        MakerNoteOriginalOffset = MakerNoteOffset - OffsetSchemaValue;
      }
      else MakerNoteOriginalOffset = 0;
    }


    private void InitIfdGpsInfoData(byte[] IfdRawData, int IfdRawDataIndex)
    {
      int TagCount = ExifReadUInt16(IfdRawData, IfdRawDataIndex);
      IfdRawDataIndex += 2;
      var GpsInfoDataIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
      TagTable[(uint)ExifIfd.GpsInfoData] = GpsInfoDataIfdTable;
      int j = 0;
      while (j < TagCount)
      {
        TagItem TempTagData = CreateTagWithReferenceToIfdRawData(IfdRawData, IfdRawDataIndex);
        AddIfNotExists(GpsInfoDataIfdTable, TempTagData);
        IfdRawDataIndex += 12;
        j++;
      }
    }


    private void InitIfdInteroperability(byte[] IfdRawData, int IfdRawDataIndex)
    {
      int TagCount = ExifReadUInt16(IfdRawData, IfdRawDataIndex);
      IfdRawDataIndex += 2;
      var InteroperabilityIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
      TagTable[(uint)ExifIfd.Interoperability] = InteroperabilityIfdTable;
      int j = 0;
      while (j < TagCount)
      {
        TagItem TempTagData = CreateTagWithReferenceToIfdRawData(IfdRawData, IfdRawDataIndex);
        AddIfNotExists(InteroperabilityIfdTable, TempTagData);
        IfdRawDataIndex += 12;
        j++;
      }
    }


    private void InitIfdThumbnailData(byte[] IfdRawData, int IfdRawDataIndex)
    {
      int ThumbnailImageDataOffset = 0;
      ThumbnailByteCount = 0;
      ThumbnailImage = null;
      int TagCount = ExifReadUInt16(IfdRawData, IfdRawDataIndex);
      IfdRawDataIndex += 2;
      var ThumbnailDataIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
      TagTable[(uint)ExifIfd.ThumbnailData] = ThumbnailDataIfdTable;
      int j = 0;
      while (j < TagCount)
      {
        TagItem TempTagData = CreateTagWithReferenceToIfdRawData(IfdRawData, IfdRawDataIndex);
        if (AddIfNotExists(ThumbnailDataIfdTable, TempTagData))
        {
          if (TempTagData.TagId == ExifTagId.JpegInterchangeFormat)
          {
            ThumbnailImageDataOffset = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
          }
          else if (TempTagData.TagId == ExifTagId.JpegInterchangeFormatLength)
          {
            ThumbnailByteCount = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
          }
        }
        IfdRawDataIndex += 12;
        j++;
      }

      if (ThumbnailImageDataOffset > 0)
      {
        GetOutsourcedData(ThumbnailImageDataOffset, ThumbnailByteCount, out ThumbnailImage, out ThumbnailStartIndex);
      }
    }


    private void GetNextIfd(int IfdOffset, out byte[] IfdRawData, out int IfdRawDataIndex)
    {
      if (IfdOffset == 0)
      {
        // IFD does not exist. Create a dummy IFD table.
        IfdRawData = new byte[2];
        IfdRawDataIndex = 0;
      }
      else if (SourceExifBlock != null)
      {
        IfdRawData = SourceExifBlock;
        IfdRawDataIndex = IfdOffset;
      }
      else
      {
        SourceExifStream.Position = IfdOffset; // + ExifBlockFilePosition;
        IfdRawDataIndex = 0;
        byte[] TagCountArray = new byte[2];
        if (SourceExifStream.Read(TagCountArray, 0, 2) == 2)
        {
          int IfdTagCount = ExifReadUInt16(TagCountArray, 0);
          int IfdSize = 2 + IfdTagCount * 12 + 4;
          IfdRawData = new byte[IfdSize];
          IfdRawData[0] = TagCountArray[0];
          IfdRawData[1] = TagCountArray[1];
          if (SourceExifStream.Read(IfdRawData, 2, IfdSize - 2) != (IfdSize - 2))
          {
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
          }
        }
        else throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
      }
    }


    // JPEG, PNG: The complete EXIF block was read and is now stored in the array "SourceExifBlock" from the array index "TifferHeaderLen" (= 8).
    // TIFF: Stream "SourceStream" is used to read the EXIF data successively. "SourceExifBlock" is "null" in this case.
    private int EvaluateExifBlock(int PrimaryDataOffset)
    {
            int TiffNextExifBlockOffset = 0;

            TagTable = new Dictionary<ExifTagId, TagItem>[ExifIfdCount];
            GetNextIfd(PrimaryDataOffset, out byte[] IfdTable, out int IfdIndex);
      InitIfdPrimaryData(IfdTable, IfdIndex, out int PrivateDataOffset, out int GpsInfoDataOffset, out int ThumbnailDataOffset);
      GetNextIfd(PrivateDataOffset, out IfdTable, out IfdIndex);
      InitIfdPrivateData(IfdTable, IfdIndex, out int InteroperabilityOffset);
      GetNextIfd(GpsInfoDataOffset, out IfdTable, out IfdIndex);
      InitIfdGpsInfoData(IfdTable, IfdIndex);
      GetNextIfd(InteroperabilityOffset, out IfdTable, out IfdIndex);
      InitIfdInteroperability(IfdTable, IfdIndex);
      if (ImageType == ImageType.Tiff)
      {
        // TIFF files don't have a thumbnail image, but they may have multiple images
        IfdTable = new byte[2];
        InitIfdThumbnailData(IfdTable, 0); // Set empty tag table for IFD ThumbnailData
        TiffNextExifBlockOffset = ThumbnailDataOffset;
      }
      else
      {
        // In JPEG files "NextImageOffset" is the offset to the thumbnail image 
        GetNextIfd(ThumbnailDataOffset, out IfdTable, out IfdIndex);
        InitIfdThumbnailData(IfdTable, IfdIndex);
      }
      return (TiffNextExifBlockOffset);
    }


    private bool AddIfNotExists(Dictionary<ExifTagId, TagItem> Dict, TagItem TagData)
    {
      try
      {
        Dict.Add(TagData.TagId, TagData);
        return (true);
      }
      catch 
      {
        // An exception occured because the tag ID already exists. In this case ignore the new tag item.
        return (false);
      } 
    }


    // Compare "Array1" from index "StartIndex1" with "Array2".
    private bool CompareArrays(byte[] Array1, int StartIndex1, byte[] Array2)
    {
      if (Array1.Length >= (StartIndex1 + Array2.Length))
      {
        bool IsEqual = true;
        int i = StartIndex1;
        foreach (byte b in Array2)
        {
          if (Array1[i] != b)
          {
            IsEqual = false;
            break;
          }
          i++;
        }
        return (IsEqual);
      }
      else return (false);
    }


    // Compare "Array1" from index 0 to index "Array2.Length - 1" with "Array2".
    // If "Array1" is smaller than "Array2", "false" is returned.
    private bool ArrayStartsWith(byte[] Array1, int Array1Length, byte[] Array2)
    {
      if (Array1Length >= Array2.Length)
      {
        bool IsEqual = true;
        int i = 0;
        foreach (byte b in Array2)
        {
          if (Array1[i] != b)
          {
            IsEqual = false;
            break;
          }
          i++;
        }
        return (IsEqual);
      }
      else return (false);
    }


    private bool ReadUintElement(TagItem t, int ElementIndex, out uint Value)
    {
      bool Success = false;

      if ((ElementIndex >= 0) && (ElementIndex < t.ValueCount))
      {
        switch (t.TagType)
        {
          case ExifTagType.Byte:
            Value = t.ValueData[t.ValueIndex + ElementIndex];
            Success = true;
            break;
          case ExifTagType.UShort:
            Value = ExifReadUInt16(t.ValueData, t.ValueIndex + (ElementIndex << 1));
            Success = true;
            break;
          case ExifTagType.ULong:
          case ExifTagType.SLong:
            Value = ExifReadUInt32(t.ValueData, t.ValueIndex + (ElementIndex << 2));
            Success = true;
            break;
          default:
            Value = 0;
            break;
        }
      }
      else Value = 0;
      return (Success);
    }


    private bool WriteUintElement(TagItem t, int ElementIndex, uint Value)
    {
      bool Success = false;

      if (t != null)
      {
        if (t.TagType == ExifTagType.Byte)
        {
          t.ValueData[t.ValueIndex + ElementIndex] = (byte)Value;
        }
        else if (t.TagType == ExifTagType.UShort)
        {
          ExifWriteUInt16(t.ValueData, t.ValueIndex + (ElementIndex << 1), (ushort)Value);
        }
        else
        {
          ExifWriteUInt32(t.ValueData, t.ValueIndex + (ElementIndex << 2), Value);
        }
        Success = true;
      }
      return (Success);
    }


    private bool GetTagValueWithIdCode(ExifTag TagSpec, out string Value, ushort CodePage)
    {
            bool Success = false, IsUtf16Coded = false, IsAsciiCoded = false;
            int i, j;

      Value = null;
      if (GetTagItem(TagSpec, out TagItem t) && (t.TagType == ExifTagType.Undefined) && (t.ValueCount >= IdCodeLength))
      {
        if (CompareArrays(t.ValueData, t.ValueIndex, IdCodeUtf16))
        {
          IsUtf16Coded = true;
        }
        else if (CompareArrays(t.ValueData, t.ValueIndex, IdCodeAscii) || CompareArrays(t.ValueData, t.ValueIndex, IdCodeDefault))
        {
          IsAsciiCoded = true;
        }

        i = t.ValueCount - IdCodeLength;
        j = t.ValueIndex + IdCodeLength;
        if (IsUtf16Coded)
        {
          // The parameter "CodePage" is ignored in this case.
          // Remove all null terminating characters. Here a null terminating character consists of 2 zero-bytes.
          while ((i >= 2) && (t.ValueData[j + i - 2] == 0) && (t.ValueData[j + i - 1] == 0))
          {
            i -= 2;
          }
          if (ByteOrder == ExifByteOrder.BigEndian)
          {
            CodePage = 1201; // UTF16BE
          }
          else CodePage = 1200; // UTF16LE
          Value = Encoding.GetEncoding(CodePage).GetString(t.ValueData, j, i);
          Success = true;
        }
        else if (IsAsciiCoded)
        {
          // Remove all null terminating characters.
          while ((i >= 1) && (t.ValueData[j + i - 1] == 0))
          {
            i--;
          }
          if ((CodePage == 1200) || (CodePage == 1201)) // UTF16LE or UTF16BE
          {
            CodePage = 20127; // Ignore parameter "CodePage" and overwrite it with US ASCII
          }
          Value = Encoding.GetEncoding(CodePage).GetString(t.ValueData, j, i);
          Success = true;
        }
      }

      return (Success);
    }


    private bool SetTagValueWithIdCode(ExifTag TagSpec, string Value, ushort CodePage)
    {
      int TotalByteCount, StrByteLen;
      TagItem t;
      bool Success = false;
      byte[] StringAsByteArray, RequiredIdCode;

      if ((CodePage == 1200) && (ByteOrder == ExifByteOrder.BigEndian))
      {
        CodePage = 1201; // Set code page to UTF16BE
      }
      if ((CodePage == 1201) && (ByteOrder == ExifByteOrder.LittleEndian))
      {
        CodePage = 1200; // Set code page to UTF16LE
      }
      StringAsByteArray = Encoding.GetEncoding(CodePage).GetBytes(Value);
      StrByteLen = StringAsByteArray.Length;
      TotalByteCount = IdCodeLength + StrByteLen; // The ID code is a 8 byte header
      t = PrepareTagForCompleteWriting(TagSpec, ExifTagType.Undefined, TotalByteCount);
      if (t != null)
      {
        if ((CodePage == 1200) || (CodePage == 1201))
        {
          RequiredIdCode = IdCodeUtf16;
        }
        else RequiredIdCode = IdCodeAscii;
        Array.Copy(RequiredIdCode, 0, t.ValueData, t.ValueIndex, IdCodeLength);
        Array.Copy(StringAsByteArray, 0, t.ValueData, t.ValueIndex + IdCodeLength, StrByteLen);
        Success = true;
      }
      return (Success);
    }


    private bool ReadURatElement(TagItem t, int ElementIndex, out uint Numer, out uint Denom)
    {
      bool Success = false;
      int i;

      if ((ElementIndex >= 0) && (ElementIndex < t.ValueCount))
      {
        if ((t.TagType == ExifTagType.SRational) || (t.TagType == ExifTagType.URational))
        {
          i = t.ValueIndex + (ElementIndex << 3);
          Numer = ExifReadUInt32(t.ValueData, i);
          Denom = ExifReadUInt32(t.ValueData, i + 4);
          Success = true;
        }
        else
        {
          Numer = 0;
          Denom = 0;
        }
      }
      else
      {
        Numer = 0;
        Denom = 0;
      }
      return (Success);
    }


    private bool WriteURatElement(TagItem t, int ElementIndex, uint Numer, uint Denom)
    {
      int i;
      bool Success = false;

      if (t != null)
      {
        i = t.ValueIndex + (ElementIndex << 3);
        ExifWriteUInt32(t.ValueData, i, Numer);
        ExifWriteUInt32(t.ValueData, i + 4, Denom);
        Success = true;
      }
      return (Success);
    }


    private bool GetTagItem(ExifTag TagSpec, out TagItem t)
    {
      ExifIfd Ifd = ExtractIfd(TagSpec);
      if ((uint)Ifd < ExifIfdCount)
      {
        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
        return (IfdTagTable.TryGetValue(ExtractTagId(TagSpec), out t));
      }
      else
      {
        t = null;
        return (false);
      }
    }


    // Prepare tag for writing a single array item. If the tag does not exist it is created.
    //
    // Parameters:
    //  "TagType":    Tag type.
    //  "ArrayIndex": Zero-based array index of the value to be written. If the array index
    //                is outside the current tag data, the tag data is automatically enlarged.
    //                If it is necessary to reallocate the tag memory, the old content is copied to the new memory.
    private TagItem PrepareTagForArrayItemWriting(ExifTag TagSpec, ExifTagType TagType, int ArrayIndex)
    {
      TagItem t = null;
      ExifIfd Ifd = ExtractIfd(TagSpec);
      if (((uint)Ifd < ExifIfdCount) && ((uint)ArrayIndex < MaxTagValueCount))
      {
        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
        if (IfdTagTable.TryGetValue(ExtractTagId(TagSpec), out t))
        {
          int ValueCount = t.ValueCount;
          if (ArrayIndex >= ValueCount)
          {
            ValueCount = ArrayIndex + 1;
          }
          t.SetTagTypeAndValueCount(TagType, ValueCount, true);
        }
        else
        {
          int ValueCount = ArrayIndex + 1;
          t = new TagItem(TagSpec, TagType, ValueCount);
          IfdTagTable.Add(t.TagId, t);
        }
      }
      return (t);
    }


    // Prepare tag for a complete writing of the tag content. The tag content is undefined after calling this method.
    //
    // Parameters:
    //  "TagType":    New tag type. The tag type must be valid, because it is not checked in this method!
    //  "ValueCount": New number of array elements that the tag should contain. Must be equal or greater than 1.
    //                If it is necessary to reallocate the tag memory, the old content is not copied.
    private TagItem PrepareTagForCompleteWriting(ExifTag TagSpec, ExifTagType TagType, int ValueCount)
    {
      TagItem t = null;

      ExifIfd Ifd = ExtractIfd(TagSpec);
      if (((uint)Ifd < ExifIfdCount) && ((uint)ValueCount <= MaxTagValueCount))
      {
        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
        if (IfdTagTable.TryGetValue(ExtractTagId(TagSpec), out t))
        {
          t.SetTagTypeAndValueCount(TagType, ValueCount, false);
        }
        else
        {
          t = new TagItem(TagSpec, TagType, ValueCount);
          IfdTagTable.Add(t.TagId, t);
        }
#if DEBUG
        int k = t.ValueIndex + t.AllocatedByteCount;
        for (int i = t.ValueIndex; i < k; i++)
        {
          t.ValueData[i] = 0xcc;
        }
#endif
      }
      return (t);
    }


    private static int CalculateTwoDigitDecNumber(byte[] ByteArr, int Index)
    {
      int d1, d2, Value;

      Value = -1;
      d1 = ByteArr[Index];
      d2 = ByteArr[Index + 1];
      if ((d1 >= '0') && (d1 <= '9') && (d2 >= '0') && (d2 <= '9'))
      {
        Value = (d1 - 0x30) * 10 + (d2 - 0x30);
      }
      return (Value);
    }


    // "Value": Number from 0 to 99.
    private static void ConvertTwoDigitNumberToByteArr(byte[] ByteArr, ref int Index, int Value)
    {
      ByteArr[Index] = (byte)((Value / 10) + 0x30);
      Index++;
      ByteArr[Index] = (byte)((Value % 10) + 0x30);
      Index++;
    }


    private void ClearIfd_Unchecked(ExifIfd Ifd)
    {
      TagTable[(uint)Ifd].Clear();
    }


    private void RemoveAllTagsFromIfdPrimaryData()
    {
      if (ImageType == ImageType.Tiff)
      {
        // In TIFF images the IFD PrimaryData contains internal tags which must not be removed otherwise
        // the image will be damaged.
        // In TIFF images the XMP and IPTC block are stored as tags within the EXIF block. Therefore these
        // tags are not removed, too.
        Dictionary<ExifTagId, TagItem> PrimaryDataIfdTable = TagTable[(uint)ExifIfd.PrimaryData];
        List<ExifTagId> TagIdsToBeRemoved = new List<ExifTagId>(PrimaryDataIfdTable.Count);
        foreach (ExifTagId tid in PrimaryDataIfdTable.Keys)
        {
          if (!IsInternalTiffTag(tid) && !IsMetaDataTiffTag(tid))
          {
            TagIdsToBeRemoved.Add(tid);
          }
        }
        foreach (ExifTagId tid in TagIdsToBeRemoved)
        {
          PrimaryDataIfdTable.Remove(tid);
        }
      }
      else
      {
        ClearIfd_Unchecked(ExifIfd.PrimaryData);
      }
    }


    private void RemoveThumbnailImage_Internal()
    {
      ThumbnailImage = null;
      ThumbnailStartIndex = 0;
      ThumbnailByteCount = 0;
    }


    private bool GetGpsCoordinateHelper(out GeoCoordinate Value, ExifTag ValueTag, ExifTag RefTag, char Cp1, char Cp2)
    {
      bool Success = false;
            char CardinalPoint;

            if (GetTagValue(ValueTag, out ExifRational Deg, 0) && Deg.IsValid() &&
          GetTagValue(ValueTag, out ExifRational Min, 1) && Min.IsValid() &&
          GetTagValue(ValueTag, out ExifRational Sec, 2) && Sec.IsValid() &&
          GetTagValue(RefTag, out string Ref, StrCoding.Utf8) && (Ref.Length == 1))
      {
        CardinalPoint = Ref[0];
        if ((CardinalPoint == Cp1) || (CardinalPoint == Cp2))
        {
          Value.Degree = ExifRational.ToDecimal(Deg);
          Value.Minute = ExifRational.ToDecimal(Min);
          Value.Second = ExifRational.ToDecimal(Sec);
          Value.CardinalPoint = CardinalPoint;
          Success = true;
        }
        else Value = new GeoCoordinate();
      }
      else Value = new GeoCoordinate();
      return (Success);
    }


    private bool SetGpsCoordinateHelper(GeoCoordinate Value, ExifTag ValueTag, ExifTag RefTag, char Cp1, char Cp2)
    {
      bool Success = false;

      ExifRational Deg = ExifRational.FromDecimal(Value.Degree);
      ExifRational Min = ExifRational.FromDecimal(Value.Minute);
      ExifRational Sec = ExifRational.FromDecimal(Value.Second);
      if (SetTagValue(ValueTag, Deg, ExifTagType.URational, 0) &&
          SetTagValue(ValueTag, Min, ExifTagType.URational, 1) &&
          SetTagValue(ValueTag, Sec, ExifTagType.URational, 2) &&
          ((Value.CardinalPoint == Cp1) || (Value.CardinalPoint == Cp2)) &&
          SetTagValue(RefTag, Value.CardinalPoint.ToString(), StrCoding.Utf8))
      {
        Success = true;
      }
      return (Success);
    }


    private bool GetDateAndTimeWithMillisecHelper(out DateTime Value, ExifTag DateAndTimeTag, ExifTag MillisecTag)
    {
      bool Success = false;
      if (GetTagValue(DateAndTimeTag, out Value))
      {
        Success = true;
        if (GetTagValue(MillisecTag, out string SubSec, StrCoding.Utf8))
        {
          string s = SubSec;
          int len = s.Length;
          if (len > 3) s = s.Substring(0, 3);
          if (int.TryParse(s, out int MilliSec) && (MilliSec >= 0))
          {
            if (len == 1) MilliSec *= 100;
            else if (len == 2) MilliSec *= 10;
            Value = Value.AddMilliseconds(MilliSec);
          }
        }
      }
      return (Success);
    }


    private bool SetDateAndTimeWithMillisecHelper(DateTime Value, ExifTag DateAndTimeTag, ExifTag MillisecTag)
    {
      bool Success = false;
      if (SetTagValue(DateAndTimeTag, Value))
      {
        Success = true;
        int MilliSec = Value.Millisecond;
        if ((MilliSec != 0) || TagExists(MillisecTag))
        {
          string s = MilliSec.ToString("000"); // Write exactly 3 decimal digits
          Success = Success && SetTagValue(MillisecTag, s, StrCoding.Utf8);
        }
      }
      return (Success);
    }


    private static readonly ExifTagId[] InternalTiffTags = new ExifTagId[] {
      ExifTagId.ImageWidth, ExifTagId.ImageLength, ExifTagId.BitsPerSample,
      ExifTagId.Compression, ExifTagId.PhotometricInterpretation, ExifTagId.Threshholding, ExifTagId.CellWidth, ExifTagId.CellLength,
      ExifTagId.FillOrder, ExifTagId.StripOffsets, ExifTagId.SamplesPerPixel, ExifTagId.RowsPerStrip, ExifTagId.StripByteCounts,
      ExifTagId.MinSampleValue, ExifTagId.MaxSampleValue, ExifTagId.PlanarConfiguration, ExifTagId.FreeOffsets, ExifTagId.FreeByteCounts,
      ExifTagId.GrayResponseUnit, ExifTagId.GrayResponseCurve, ExifTagId.T4Options, ExifTagId.T6Options, ExifTagId.TransferFunction,
      ExifTagId.Predictor, ExifTagId.WhitePoint, ExifTagId.PrimaryChromaticities, ExifTagId.ColorMap, ExifTagId.HalftoneHints,
      ExifTagId.TileWidth, ExifTagId.TileLength, ExifTagId.TileOffsets, ExifTagId.TileByteCounts, ExifTagId.ExtraSamples,
      ExifTagId.SampleFormat, ExifTagId.SMinSampleValue, ExifTagId.SMaxSampleValue, ExifTagId.TransferRange,
      ExifTagId.YCbCrCoefficients, ExifTagId.YCbCrSubSampling, ExifTagId.YCbCrPositioning, ExifTagId.ReferenceBlackWhite,
      (ExifTagId)0x0200, (ExifTagId)0x0201, (ExifTagId)0x0202, (ExifTagId)0x0203, (ExifTagId)0x0205, (ExifTagId)0x0206,
      (ExifTagId)0x0207, (ExifTagId)0x0208, (ExifTagId)0x0209
      };
    private static BitArray InternalTiffTagsBitArray;


    private static void InitInternalTiffTags()
    {
      InternalTiffTagsBitArray = new BitArray(0x0215, false);
      int len = InternalTiffTagsBitArray.Length;
      foreach (ExifTagId TagId in InternalTiffTags)
      {
        int TagIdInt = (int)TagId;
        if (TagIdInt >= len)
        {
          len = len << 1;
          if (TagIdInt >= len) len = TagIdInt + 1;
          InternalTiffTagsBitArray.Length = len;
        }
        InternalTiffTagsBitArray.Set(TagIdInt, true);
      }
    }


    // Only for TIFF images
    private static bool IsInternalTiffTag(ExifTagId TagId)
    {
      if (InternalTiffTagsBitArray == null)
      {
        InitInternalTiffTags();
      }
      int TagIdInt = (int)TagId;
      if (TagIdInt < InternalTiffTagsBitArray.Length)
      {
        return (InternalTiffTagsBitArray.Get(TagIdInt));
      }
      return (false);
    }


    // Only for TIFF images
    private static bool IsMetaDataTiffTag(ExifTagId TagId)
    {
      return ((TagId == ExifTagId.XmpMetadata) || (TagId == ExifTagId.IptcMetadata));
    }


    private void SwapByteOrderOfTagData(ExifIfd Ifd, TagItem t)
    {
      // Special tag IDs
      if (Ifd == ExifIfd.PrimaryData)
      {
        // There are meta data tags where the byte order must not be changed. These tags are only used in TIFF files.
        if (IsMetaDataTiffTag(t.TagId)) return;
      }
      else if (Ifd == ExifIfd.PrivateData)
      {
        if ((t.TagId == ExifTagId.UserComment) && (t.TagType == ExifTagType.Undefined))
        {
          // This tag is coded UTF16LE or UTF16BE depending on the byte order.
          int k = t.ValueIndex + 8; // Skip 8 byte ID code which is not UTF16 coded
          int Utf16CharCount = (t.ValueCount - 8) / 2;
          for (int i = 0; i < Utf16CharCount; i++)
          {
            Swap2ByteValue(t.ValueData, k);
            k += 2;
          }
          return;
        }
      }

      switch (t.TagType)
      {
        case ExifTagType.UShort:
        case ExifTagType.SShort:
          int k = t.ValueIndex;
          for (int i = 0; i < t.ValueCount; i++)
          {
            Swap2ByteValue(t.ValueData, k);
            k += 2;
          }
          break;

        case ExifTagType.ULong:
        case ExifTagType.SLong:
        case ExifTagType.Float:
          k = t.ValueIndex;
          for (int i = 0; i < t.ValueCount; i++)
          {
            Swap4ByteValue(t.ValueData, k);
            k += 4;
          }
          break;

        case ExifTagType.URational:
        case ExifTagType.SRational:
          k = t.ValueIndex;
          for (int i = 0; i < t.ValueCount; i++)
          {
            Swap4ByteValue(t.ValueData, k); // Numerator of the rational number
            k += 4;
            Swap4ByteValue(t.ValueData, k); // Denominator of the rational number
            k += 4;
          }
          break;

        case ExifTagType.Double:
          k = t.ValueIndex;
          for (int i = 0; i < t.ValueCount; i++)
          {
            Array.Reverse(t.ValueData, k, 8);
            k += 8;
          }
          break;
      }
    }


    private void Swap2ByteValue(byte[] b, int i)
    {
      byte k = b[i];
      b[i] = b[i + 1];
      b[i + 1] = k;
    }


    private void Swap4ByteValue(byte[] b, int i)
    {
      byte k = b[i];
      b[i] = b[i + 3];
      b[i + 3] = k;
      k = b[i + 1];
      b[i + 1] = b[i + 2];
      b[i + 2] = k;
    }


    private TagItem CopyTagItemDeeply(ExifIfd Ifd, TagItem TagItemToBeCopied, bool SwapByteOrder)
    {
      TagItem NewTag = new TagItem(TagItemToBeCopied.TagId, TagItemToBeCopied.TagType, TagItemToBeCopied.ValueCount);
      Array.Copy(TagItemToBeCopied.ValueData, TagItemToBeCopied.ValueIndex, NewTag.ValueData, 0, NewTag.ByteCount);
      if (SwapByteOrder)
      {
        SwapByteOrderOfTagData(Ifd, NewTag);
      }
      return (NewTag);
    }


    private class TagItem
    {
      public ExifTagId TagId;
      public ExifTagType TagType;
      public int ValueCount; // Number of values of the tag. In general this is not the number of bytes of the tag!
      public byte[] ValueData; // Array which contains the tag data. There may be other data stored in this array.
      public int ValueIndex; // Array index from which the tag data starts.
      public int AllocatedByteCount; // Number of bytes which are allocated for the tag data. 
                                     // This number can be greater than the required number of bytes.
      public int ByteCount // The number of bytes required to store the tag data, i. e. the number of bytes actually used for the tag data.
      {
        get
        {
          return (ExifData.GetTagByteCount(TagType, ValueCount));
        }
      }
      public int OriginalDataOffset; // Offset of outsourced data in the image file. 0 = Tag does not have outsourced data or
                                     // tag was created after loading.


      public TagItem(ExifTag TagSpec, ExifTagType _TagType, int _ValueCount) :
        this(ExtractTagId(TagSpec), _TagType, _ValueCount)
      {
      }


      public TagItem(ExifTagId _TagId, ExifTagType _TagType, int _ValueCount)
      {
        TagId = _TagId;
        TagType = _TagType;
        ValueCount = _ValueCount;
        int RequiredByteCount = GetTagByteCount(_TagType, _ValueCount);
        ValueData = AllocTagMemory(RequiredByteCount);
        AllocatedByteCount = ValueData.Length;
        ValueIndex = 0;
      }


      public TagItem(ExifTagId _TagId, ExifTagType _TagType, int _ValueCount, byte[] _ValueArray, int _ValueIndex, 
        int _AllocatedByteCount, int _OriginalDataOffset = 0)
      {
        TagId = _TagId;
        TagType = _TagType;
        ValueCount = _ValueCount;
        ValueData = _ValueArray;
        ValueIndex = _ValueIndex;
        AllocatedByteCount = _AllocatedByteCount;
        OriginalDataOffset = _OriginalDataOffset;
      }


      public static byte[] AllocTagMemory(int RequiredByteCount)
      {
        const int MinByteCount = 32;
        int NewByteCount = RequiredByteCount;
        if (NewByteCount < MinByteCount)
        {
          NewByteCount = MinByteCount;
        }
        return (new byte[NewByteCount]);
      }


      // Set tag type and value count and reallocate the tag memory if required.
      public void SetTagTypeAndValueCount(ExifTagType _TagType, int _ValueCount, bool KeepExistingData)
      {
        int RequiredByteCount = GetTagByteCount(_TagType, _ValueCount);
        if (AllocatedByteCount < RequiredByteCount)
        {
          uint NewByteCount = (uint)AllocatedByteCount << 1; // Double the allocated byte count
          if (NewByteCount > int.MaxValue) NewByteCount = int.MaxValue;
          else if (NewByteCount < (uint)RequiredByteCount) NewByteCount = (uint)RequiredByteCount;
          byte[] NewTagData = AllocTagMemory((int)NewByteCount);
          if (KeepExistingData)
          {
            Array.Copy(ValueData, ValueIndex, NewTagData, 0, AllocatedByteCount);
          }
          AllocatedByteCount = NewTagData.Length;
          ValueData = NewTagData;
          ValueIndex = 0;
        }
        TagType = _TagType;
        ValueCount = _ValueCount;
      }
    }


    private class FlexArray
    {
      public FlexArray(int Capacity)
      {
        _Buffer = new byte[Capacity];
        _Length = 0;
      }


      public byte this[int i]
      {
        get 
        {
          return _Buffer[i]; 
        }
        set 
        {
          _Buffer[i] = value; 
        }
      }


      private byte[] _Buffer;
      public byte[] Buffer
      {
        get { return (_Buffer); }
      }


      private int _Length;
      public int Length
      {
        get { return (_Length); }
        set
        {
          if (value > _Buffer.Length)
          {
            uint k = (uint)_Buffer.Length;
            k = k << 1;
            if (k > int.MaxValue)
            {
              k = int.MaxValue;
            }
            else if ((uint)value > k)
            {
              k = (uint)value;
            }
            Array.Resize(ref _Buffer, (int)k);
          }
          _Length = value;
        }
      }
    }
    #endregion
  }


  // IFD Constants. These constants are used as array indexes for the array "TagTable".
  public enum ExifIfd
  {
    PrimaryData = 0,
    PrivateData = 1,
    GpsInfoData = 2,
    Interoperability = 3,
    ThumbnailData = 4
  }


  // Tag specification constants: Composition of IFD and tag ID.
  public enum ExifTag
  {
    // IFD Primary Data
    NewSubfileType = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.NewSubfileType,
    SubfileType = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.SubfileType,
    ImageWidth = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.ImageWidth,
    ImageLength = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.ImageLength,
    BitsPerSample = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.BitsPerSample,
    Compression = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Compression,
    PhotometricInterpretation = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.PhotometricInterpretation,
    Threshholding = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Threshholding,
    CellWidth = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.CellWidth,
    CellLength = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.CellLength,
    FillOrder = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.FillOrder,
    DocumentName = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.DocumentName,
    ImageDescription = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.ImageDescription,
    Make = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Make,
    Model = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Model,
    StripOffsets = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.StripOffsets,
    Orientation = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Orientation,
    SamplesPerPixel = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.SamplesPerPixel,
    RowsPerStrip = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.RowsPerStrip,
    StripByteCounts = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.StripByteCounts,
    MinSampleValue = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.MinSampleValue,
    MaxSampleValue = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.MaxSampleValue,
    XResolution = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.XResolution,
    YResolution = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.YResolution,
    PlanarConfiguration = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.PlanarConfiguration,
    PageName = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.PageName,
    XPosition = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.XPosition,
    YPosition = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.YPosition,
    FreeOffsets = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.FreeOffsets,
    FreeByteCounts = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.FreeByteCounts,
    GrayResponseUnit = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.GrayResponseUnit,
    GrayResponseCurve = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.GrayResponseCurve,
    T4Options = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.T4Options,
    T6Options = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.T6Options,
    ResolutionUnit = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.ResolutionUnit,
    PageNumber = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.PageNumber,
    TransferFunction = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.TransferFunction,
    Software = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Software,
    DateTime = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.DateTime,
    Artist = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Artist,
    HostComputer = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.HostComputer,
    Predictor = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Predictor,
    WhitePoint = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.WhitePoint,
    PrimaryChromaticities = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.PrimaryChromaticities,
    ColorMap = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.ColorMap,
    HalftoneHints = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.HalftoneHints,
    TileWidth = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.TileWidth,
    TileLength = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.TileLength,
    TileOffsets = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.TileOffsets,
    TileByteCounts = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.TileByteCounts,
    InkSet = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.InkSet,
    InkNames = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.InkNames,
    NumberOfInks = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.NumberOfInks,
    DotRange = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.DotRange,
    TargetPrinter = 0x0151,
    ExtraSamples = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.ExtraSamples,
    SampleFormat = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.SampleFormat,
    SMinSampleValue = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.SMinSampleValue,
    SMaxSampleValue = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.SMaxSampleValue,
    TransferRange = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.TransferRange,
    YCbCrCoefficients = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.YCbCrCoefficients,
    YCbCrSubSampling = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.YCbCrSubSampling,
    YCbCrPositioning = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.YCbCrPositioning,
    ReferenceBlackWhite = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.ReferenceBlackWhite,
    XmpMetadata = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.XmpMetadata,
    Copyright = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Copyright,
    IptcMetadata = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.IptcMetadata,
    ExifIfdPointer = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.ExifIfdPointer,
    GpsInfoIfdPointer = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.GpsInfoIfdPointer,
    XpTitle = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.XpTitle,
    XpComment = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.XpComment,
    XpAuthor = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.XpAuthor,
    XpKeywords = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.XpKeywords,
    XpSubject = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.XpSubject,
    PrimaryDataPadding = (ExifIfd.PrimaryData << ExifData.IfdShift) | ExifTagId.Padding,

    // IFD Private Data
    ExposureTime = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ExposureTime,
    FNumber = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FNumber,
    ExposureProgram = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ExposureProgram,
    SpectralSensitivity = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SpectralSensitivity,
    IsoSpeedRatings = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.IsoSpeedRatings,
    PhotographicSensitivity = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.PhotographicSensitivity,
    Oecf = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.Oecf,
    SensitivityType = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SensitivityType,
    StandardOutputSensitivity = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.StandardOutputSensitivity,
    RecommendedExposureIndex = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.RecommendedExposureIndex,
    IsoSpeed = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.IsoSpeed,
    IsoSpeedLatitudeyyy = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.IsoSpeedLatitudeyyy,
    IsoSpeedLatitudezzz = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.IsoSpeedLatitudezzz,
    ExifVersion = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ExifVersion,
    DateTimeOriginal = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.DateTimeOriginal,
    DateTimeDigitized = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.DateTimeDigitized,
    OffsetTime = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.OffsetTime,
    OffsetTimeOriginal = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.OffsetTimeOriginal,
    OffsetTimeDigitized = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.OffsetTimeDigitized,
    ComponentsConfiguration = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ComponentsConfiguration,
    CompressedBitsPerPixel = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.CompressedBitsPerPixel,
    ShutterSpeedValue = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ShutterSpeedValue,
    ApertureValue = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ApertureValue,
    BrightnessValue = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.BrightnessValue,
    ExposureBiasValue = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ExposureBiasValue,
    MaxApertureValue = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.MaxApertureValue,
    SubjectDistance = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SubjectDistance,
    MeteringMode = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.MeteringMode,
    LightSource = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.LightSource,
    Flash = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.Flash,
    FocalLength = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FocalLength,
    SubjectArea = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SubjectArea,
    MakerNote = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.MakerNote,
    UserComment = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.UserComment,
    SubsecTime = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SubsecTime,
    SubsecTimeOriginal = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SubsecTimeOriginal,
    SubsecTimeDigitized = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SubsecTimeDigitized,
    FlashPixVersion = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FlashPixVersion,
    ColorSpace = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ColorSpace,
    PixelXDimension = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.PixelXDimension,
    PixelYDimension = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.PixelYDimension,
    RelatedSoundFile = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.RelatedSoundFile,
    InteroperabilityIfdPointer = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.InteroperabilityIfdPointer,
    FlashEnergy = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FlashEnergy,
    SpatialFrequencyResponse = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SpatialFrequencyResponse,
    FocalPlaneXResolution = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FocalPlaneXResolution,
    FocalPlaneYResolution = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FocalPlaneYResolution,
    FocalPlaneResolutionUnit = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FocalPlaneResolutionUnit,
    SubjectLocation = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SubjectLocation,
    ExposureIndex = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ExposureIndex,
    SensingMethod = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SensingMethod,
    FileSource = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FileSource,
    SceneType = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SceneType,
    CfaPattern = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.CfaPattern,
    CustomRendered = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.CustomRendered,
    ExposureMode = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ExposureMode,
    WhiteBalance = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.WhiteBalance,
    DigitalZoomRatio = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.DigitalZoomRatio,
    FocalLengthIn35mmFilm = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.FocalLengthIn35mmFilm,
    SceneCaptureType = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SceneCaptureType,
    GainControl = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.GainControl,
    Contrast = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.Contrast,
    Saturation = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.Saturation,
    Sharpness = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.Sharpness,
    DeviceSettingDescription = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.DeviceSettingDescription,
    SubjectDistanceRange = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.SubjectDistanceRange,
    ImageUniqueId = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.ImageUniqueId,
    CameraOwnerName = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.CameraOwnerName,
    BodySerialNumber = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.BodySerialNumber,
    LensSpecification = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.LensSpecification,
    LensMake = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.LensMake,
    LensModel = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.LensModel,
    LensSerialNumber = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.LensSerialNumber,
    PrivateDataPadding = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.Padding,
    OffsetSchema = (ExifIfd.PrivateData << ExifData.IfdShift) | ExifTagId.OffsetSchema,

    // IFD GPS Data
    GpsVersionId = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsVersionId,
    GpsLatitudeRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsLatitudeRef,
    GpsLatitude = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsLatitude,
    GpsLongitudeRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsLongitudeRef,
    GpsLongitude = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsLongitude,
    GpsAltitudeRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsAltitudeRef,
    GpsAltitude = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsAltitude,
    GpsTimeStamp = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsTimestamp,
    GpsSatellites = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsSatellites,
    GpsStatus = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsStatus,
    GpsMeasureMode = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsMeasureMode,
    GpsDop = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDop,
    GpsSpeedRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsSpeedRef,
    GpsSpeed = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsSpeed,
    GpsTrackRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsTrackRef,
    GpsTrack = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsTrack,
    GpsImgDirectionRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsImgDirectionRef,
    GpsImgDirection = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsImgDirection,
    GpsMapDatum = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsMapDatum,
    GpsDestLatitudeRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDestLatitudeRef,
    GpsDestLatitude = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDestLatitude,
    GpsDestLongitudeRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDestLongitudeRef,
    GpsDestLongitude = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDestLongitude,
    GpsDestBearingRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDestBearingRef,
    GpsDestBearing = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDestBearing,
    GpsDestDistanceRef = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDestDistanceRef,
    GpsDestDistance = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDestDistance,
    GpsProcessingMethod = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsProcessingMethod,
    GpsAreaInformation = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsAreaInformation,
    GpsDateStamp = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDateStamp,
    GpsDifferential = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsDifferential,
    GpsHPositioningError = (ExifIfd.GpsInfoData << ExifData.IfdShift) | ExifTagId.GpsHPositioningError,

    // IFD Interoperability
    InteroperabilityIndex = (ExifIfd.Interoperability << ExifData.IfdShift) | ExifTagId.InteroperabilityIndex,
    InteroperabilityVersion = (ExifIfd.Interoperability << ExifData.IfdShift) | ExifTagId.InteroperabilityVersion,

    // IFD Thumbnail Data
    ThumbnailImageWidth = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.ImageWidth,
    ThumbnailImageLength = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.ImageLength,
    ThumbnailCompression = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.Compression,
    ThumbnailXResolution = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.XResolution,
    ThumbnailYResolution = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.YResolution,
    ThumbnailResolutionUnit = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.ResolutionUnit,
    ThumbnailOrientation = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.Orientation,
    JpegInterchangeFormat = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.JpegInterchangeFormat,
    JpegInterchangeFormatLength = (ExifIfd.ThumbnailData << ExifData.IfdShift) | ExifTagId.JpegInterchangeFormatLength,
  }


  // Tag ID constants.
  public enum ExifTagId
  {
    // IFD Primary Data, some tags are also used in IFD Thumbnail Data
    NewSubfileType = 0x00FE,
    SubfileType = 0x00FF,
    ImageWidth = 0x0100,
    ImageLength = 0x0101,
    BitsPerSample = 0x0102,
    Compression = 0x0103,
    PhotometricInterpretation = 0x0106,
    Threshholding = 0x0107,
    CellWidth = 0x0108,
    CellLength = 0x0109,
    FillOrder = 0x010a,
    DocumentName = 0x010d,
    ImageDescription = 0x010e,
    Make = 0x010f,
    Model = 0x0110,
    StripOffsets = 0x0111,
    Orientation = 0x0112,
    SamplesPerPixel = 0x0115,
    RowsPerStrip = 0x0116,
    StripByteCounts = 0x0117,
    MinSampleValue = 0x0118,
    MaxSampleValue = 0x0119,
    XResolution = 0x011a,
    YResolution = 0x011b,
    PlanarConfiguration = 0x011c,
    PageName = 0x011d,
    XPosition = 0x011e,
    YPosition = 0x011f,
    FreeOffsets = 0x0120,
    FreeByteCounts = 0x0121,
    GrayResponseUnit = 0x0122,
    GrayResponseCurve = 0x0123,
    T4Options = 0x0124,
    T6Options = 0x0125,
    ResolutionUnit = 0x0128,
    PageNumber = 0x0129,
    TransferFunction = 0x012d,
    Software = 0x0131,
    DateTime = 0x0132,
    Artist = 0x013b,
    HostComputer = 0x013c,
    Predictor = 0x013d,
    WhitePoint = 0x013e,
    PrimaryChromaticities = 0x013f,
    ColorMap = 0x0140,
    HalftoneHints = 0x0141,
    TileWidth = 0x0142,
    TileLength = 0x0143,
    TileOffsets = 0x0144,
    TileByteCounts = 0x0145,
    InkSet = 0x014c,
    InkNames = 0x014d,
    NumberOfInks = 0x014e,
    DotRange = 0x0150,
    TargetPrinter = 0x0151,
    ExtraSamples = 0x0152,
    SampleFormat = 0x0153,
    SMinSampleValue = 0x0154,
    SMaxSampleValue = 0x0155,
    TransferRange = 0x0156,
    YCbCrCoefficients = 0x0211,
    YCbCrSubSampling = 0x0212,
    YCbCrPositioning = 0x0213,
    ReferenceBlackWhite = 0x0214,
    XmpMetadata = 0x02bc, // XMP block within a TIFF file
    Copyright = 0x8298,
    IptcMetadata = 0x83bb, // IPTC block within a TIFF file
    ExifIfdPointer = 0x8769,
    GpsInfoIfdPointer = 0x8825,
    XpTitle = 0x9c9b,
    XpComment = 0x9c9c,
    XpAuthor = 0x9c9d,
    XpKeywords = 0x9c9e,
    XpSubject = 0x9c9f,
    Padding = 0xea1c,

    // IFD Thumbnail Data
    JpegInterchangeFormat = 0x0201,
    JpegInterchangeFormatLength = 0x0202,

    // IFD Private Data
    ExposureTime = 0x829a,
    FNumber = 0x829d,
    ExposureProgram = 0x8822,
    SpectralSensitivity = 0x8824,
    IsoSpeedRatings = 0x8827,
    PhotographicSensitivity = 0x8827, // Tag was renamed from "IsoSpeedRatings" to this name
    Oecf = 0x8828,
    SensitivityType = 0x8830,
    StandardOutputSensitivity = 0x8831,
    RecommendedExposureIndex = 0x8832,
    IsoSpeed = 0x8833,
    IsoSpeedLatitudeyyy = 0x8834,
    IsoSpeedLatitudezzz = 0x8835,
    ExifVersion = 0x9000,
    DateTimeOriginal = 0x9003,
    DateTimeDigitized = 0x9004,
    OffsetTime = 0x9010,
    OffsetTimeOriginal = 0x9011,
    OffsetTimeDigitized = 0x9012,
    ComponentsConfiguration = 0x9101,
    CompressedBitsPerPixel = 0x9102,
    ShutterSpeedValue = 0x9201,
    ApertureValue = 0x9202,
    BrightnessValue = 0x9203,
    ExposureBiasValue = 0x9204,
    MaxApertureValue = 0x9205,
    SubjectDistance = 0x9206,
    MeteringMode = 0x9207,
    LightSource = 0x9208,
    Flash = 0x9209,
    FocalLength = 0x920a,
    SubjectArea = 0x9214,
    MakerNote = 0x927c,
    UserComment = 0x9286,
    SubsecTime = 0x9290,
    SubsecTimeOriginal = 0x9291,
    SubsecTimeDigitized = 0x9292,
    FlashPixVersion = 0xa000,
    ColorSpace = 0xa001,
    PixelXDimension = 0xa002,
    PixelYDimension = 0xa003,
    RelatedSoundFile = 0xa004,
    InteroperabilityIfdPointer = 0xa005,
    FlashEnergy = 0xa20b,
    SpatialFrequencyResponse = 0xa20c,
    FocalPlaneXResolution = 0xa20e,
    FocalPlaneYResolution = 0xa20f,
    FocalPlaneResolutionUnit = 0xa210,
    SubjectLocation = 0xa214,
    ExposureIndex = 0xa215,
    SensingMethod = 0xa217,
    FileSource = 0xa300,
    SceneType = 0xa301,
    CfaPattern = 0xa302,
    CustomRendered = 0xa401,
    ExposureMode = 0xa402,
    WhiteBalance = 0xa403,
    DigitalZoomRatio = 0xa404,
    FocalLengthIn35mmFilm = 0xa405,
    SceneCaptureType = 0xa406,
    GainControl = 0xa407,
    Contrast = 0xa408,
    Saturation = 0xa409,
    Sharpness = 0xa40a,
    DeviceSettingDescription = 0xa40b,
    SubjectDistanceRange = 0xa40c,
    ImageUniqueId = 0xa420,
    CameraOwnerName = 0xa430,
    BodySerialNumber = 0xa431,
    LensSpecification = 0xa432,
    LensMake = 0xa433,
    LensModel = 0xa434,
    LensSerialNumber = 0xa435,
    OffsetSchema = 0xea1d,

    // IFD GPS Data
    GpsVersionId = 0x0000,
    GpsLatitudeRef = 0x0001,
    GpsLatitude = 0x0002,
    GpsLongitudeRef = 0x0003,
    GpsLongitude = 0x0004,
    GpsAltitudeRef = 0x0005,
    GpsAltitude = 0x0006,
    GpsTimestamp = 0x0007,
    GpsSatellites = 0x0008,
    GpsStatus = 0x0009,
    GpsMeasureMode = 0x000a,
    GpsDop = 0x000b,
    GpsSpeedRef = 0x000c,
    GpsSpeed = 0x000d,
    GpsTrackRef = 0x000e,
    GpsTrack = 0x000f,
    GpsImgDirectionRef = 0x0010,
    GpsImgDirection = 0x0011,
    GpsMapDatum = 0x0012,
    GpsDestLatitudeRef = 0x0013,
    GpsDestLatitude = 0x0014,
    GpsDestLongitudeRef = 0x0015,
    GpsDestLongitude = 0x0016,
    GpsDestBearingRef = 0x0017,
    GpsDestBearing = 0x0018,
    GpsDestDistanceRef = 0x0019,
    GpsDestDistance = 0x001a,
    GpsProcessingMethod = 0x001b,
    GpsAreaInformation = 0x001c,
    GpsDateStamp = 0x001d,
    GpsDifferential = 0x001e,
    GpsHPositioningError = 0x001f,

    // IFD Interoperability
    InteroperabilityIndex = 0x0001,
    InteroperabilityVersion = 0x0002
  }


  // Tag types. These constants are used as array indexes for the array "TypeByteCount".
  public enum ExifTagType
  {
    Byte = 1,
    Ascii = 2,
    UShort = 3,
    ULong = 4,
    URational = 5,
    SByte = 6, // Only for TIFFs
    Undefined = 7,
    SShort = 8, // Only for TIFFs
    SLong = 9,
    SRational = 10,
    Float = 11, // Only for TIFFs
    Double = 12 // Only for TIFFs
  }


  public enum StrCodingFormat 
  { 
    TypeAscii = 0x00000000, // Tag type is "ExifTagType.Ascii". A null terminating character is added when writing.
    TypeUndefined = 0x00010000, // Tag type is "ExifTagType.Undefined". A null terminating character is not present.
    TypeByte = 0x00020000, // Tag type is "ExifTagType.Byte". A null terminating character is added when writing.
    TypeUndefinedWithIdCode = 0x00030000 // Tag type is "ExifTagType.Undefined" and an additional ID code is present. A null terminating character is not present.
  };


  // Strings coding constants. In the lower 16 bits the code page number (1 to 65535) is coded.
  // In the higher 16 bits the EXIF tag type and additional infos are coded.
  public enum StrCoding
  {
    Utf8 = StrCodingFormat.TypeAscii | 65001, // Default value for all tags of type "ExifTagType.Ascii". 
    UsAscii = StrCodingFormat.TypeAscii | 20127,
    WestEuropeanWin = StrCodingFormat.TypeAscii | 1252,
    UsAscii_Undef = StrCodingFormat.TypeUndefined | 20127, // For the tags "ExifVersion", "FlashPixVersion" and others.			   
    Utf16Le_Byte = StrCodingFormat.TypeByte | 1200, // For the Microsoft tags "XpTitle", "XpComment", "XpAuthor", "XpKeywords" and "XpSubject".
    IdCode_Utf16 = StrCodingFormat.TypeUndefinedWithIdCode | 1200, // Default value for the tag "UserComment".
    IdCode_UsAscii = StrCodingFormat.TypeUndefinedWithIdCode | 20127,
    IdCode_WestEu = StrCodingFormat.TypeUndefinedWithIdCode | 1252
  }


  public enum ExifDateFormat
  {
    DateAndTime = 0,
    DateOnly    = 1
  }


  public struct ExifRational
  {
    public uint Numer, Denom;
    public bool Sign; // true = Negative number or negative zero

    public ExifRational(int _Numer, int _Denom)
    {
      if (_Numer < 0)
      {
        Numer = (uint)-_Numer;
        Sign = true;
      }
      else
      {
        Numer = (uint)_Numer;
        Sign = false;
      }
      if (_Denom < 0)
      {
        Denom = (uint)-_Denom;
        Sign = !Sign;
      }
      else
      {
        Denom = (uint)_Denom;
      }
    }


    public ExifRational(uint _Numer, uint _Denom, bool _Sign = false)
    {
      Numer = _Numer;
      Denom = _Denom;
      Sign = _Sign;
    }


    public bool IsNegative()
    {
      return ((Sign == true) && (Numer != 0));
    }


    public bool IsPositive()
    {
      return ((Sign == false) && (Numer != 0));
    }


    public bool IsZero()
    {
      return (Numer == 0);
    }


    public bool IsValid()
    {
      return (Denom != 0);
    }


    public new string ToString()
    {
      string Sign = "";
      if (IsNegative()) Sign = "-";
      return(Sign + Numer.ToString() + '/' + Denom.ToString());
    }


    public static decimal ToDecimal(ExifRational Value)
    {
      decimal ret = ((decimal)Value.Numer) / Value.Denom;
      if (Value.Sign) ret = -ret;
      return (ret);
    }


    public static ExifRational FromDecimal(decimal Value)
    {
      ExifRational ret;
      uint denom = 1;
      decimal numer, tempNumer;

      if (Value >= 0)
      {
        numer = Value;
        ret.Sign = false;
      }
      else
      {
        numer = -Value;
        ret.Sign = true;
      }

      if (numer >= 1e9m)
      {
        numer = Math.Truncate(numer + 0.5m);
        if (numer <= uint.MaxValue)
        {
          ret.Numer = (uint)numer;
        }
        else throw new OverflowException();
      }
      else
      {
        while (numer != decimal.Truncate(numer))
        {
          tempNumer = numer * 10;
          if ((denom <= 100000000) && (decimal.Truncate(tempNumer + 0.5m) < 1e9m))
          {
            numer = tempNumer; // The rounded numerator should be smaller than 10^9 if "Value" has decimals
            denom *= 10; // The maximum possible power of 10 for the denominator is 10^9
          }
          else break;
        }
        ret.Numer = (uint)decimal.Truncate(numer + 0.5m);
      }
      ret.Denom = denom;

      // Reduce fraction by a power of 10 if possible
      while ((ret.Denom >= 10) && ((ret.Numer % 10) == 0))
      {
        ret.Numer /= 10;
        ret.Denom /= 10;
      }
      return (ret);
    }
  }


  public struct GeoCoordinate
  {
    public decimal Degree; // Integer number: 0 ≤ Degree ≤ 90 (for latitudes) or 180 (for longitudes)
    public decimal Minute; // Integer number: 0 ≤ Minute < 60
    public decimal Second; // Fraction number: 0 ≤ Second < 60
    public char CardinalPoint; // For latitudes: 'N' or 'S'; for longitudes: 'E' or 'W'


    public static decimal ToDecimal(GeoCoordinate Value)
    {
      decimal DecimalDegree = Value.Degree + Value.Minute / 60 + Value.Second / 3600;
      if ((Value.CardinalPoint == 'S') || (Value.CardinalPoint == 'W'))
      {
        DecimalDegree = -DecimalDegree;
      }
      return (DecimalDegree);
    }


    public static GeoCoordinate FromDecimal(decimal Value, bool IsLatitude)
    {
      decimal AbsValue;
      GeoCoordinate ret;

      if (Value >= 0)
      {
        ret.CardinalPoint = IsLatitude ? 'N' : 'E';
        AbsValue = Value;
      }
      else
      {
        ret.CardinalPoint = IsLatitude ? 'S' : 'W';
        AbsValue = -Value;
      }
      ret.Degree = decimal.Truncate(AbsValue);
      decimal frac = (AbsValue - ret.Degree) * 60;
      ret.Minute = decimal.Truncate(frac);
      ret.Second = (frac - ret.Minute) * 60;
      return (ret);
    }
  }


  // Byte order of the EXIF data
  public enum ExifByteOrder { LittleEndian, BigEndian };


  [Flags]
  public enum ExifLoadOptions
  {
    CreateEmptyBlock = 0x00000001
  };


  [Flags]
  public enum ExifSaveOptions
  {
    // There are no save options at the moment
  };


  public enum ExifErrCode
  {
    InternalError,
    ImageTypeIsNotSupported,
    ImageHasUnsupportedFeatures,
    InternalImageStructureIsWrong,
    ExifBlockHasIllegalContent, // Internal image structure is OK but the EXIF block has an illegal content
    ExifDataAreTooLarge,
    ImageTypesDoNotMatch
  };


  public enum ImageFileBlock
  {
    // The values must be enumerated consecutively
    Unknown = 0, // Internal value, do not use
    Exif = 1,
    Iptc = 2,
    Xmp = 3,
    JpegComment = 4,
    PngMetaData = 5,
    PngDateChanged = 6
  };

  public enum ImageType { Unknown = 0, Jpeg, Tiff, Png };

  public class ExifException : Exception
  {
    public ExifErrCode ErrorCode;

    public ExifException(ExifErrCode _ErrorCode)
    {
      ErrorCode = _ErrorCode;
    }

    public override string Message
    {
      get
      {
        string s;
        switch (ErrorCode)
        {
          case ExifErrCode.ImageTypeIsNotSupported: s = "Image type is not supported!"; break;
          case ExifErrCode.ImageHasUnsupportedFeatures: s = "Image has unsupported features!"; break;
          case ExifErrCode.InternalImageStructureIsWrong: s = "Internal image structure is wrong!"; break;
          case ExifErrCode.ExifBlockHasIllegalContent: s = "EXIF block has an illegal content!"; break;
          case ExifErrCode.ExifDataAreTooLarge: s = "EXIF data are too large: 64 kB for JPEG files, 2 GB for TIFF files!"; break; 
          case ExifErrCode.ImageTypesDoNotMatch: s = "Image types do not match!"; break;
          default: s= "Internal error!"; break;
        }
        return (s);
      }
    }
  }
}

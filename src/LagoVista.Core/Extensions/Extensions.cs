// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c00e02ca3a92fe0bc827dbf4c8b2595e53117a6091ce28dfea19a07ab0457a70
// IndexVersion: 0
// --- END CODE INDEX META ---
#region License
/*Copyright (c) 2017, Software Logistics, LLC
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE.*/
#endregion

using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LagoVista.Core
{
    public static class StringExtensions
    {
        private static readonly char[] captials = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static readonly string[] roadTypeAbbreviations = new[] { "ALY.", "ANX.", "ARC.", "AVE.", "BCH.", "BG.", "BGS.", "BLF.", "BLFS.", "BLVD.", "BND.", "BR.", "BRG.", "BRK.", "BRKS.", "BTM.", "BYP.", "BYU.", "CIR.", "CIRS.", "CLB.", "CLF.", "CLFS.", "CMN.", "COR.", "CORS.", "CP.", "CPE.", "CRES.", "CRK.", "CRSE.", "CRST.", "CSWY.", "CT.", "CTR.", "CTRS.", "CTS.", "CURV.", "CV.", "CVS.", "CYN.", "DL.", "DM.", "DR.", "DRS.", "DV.", "EST.", "ESTS.", "EXPY.", "EXT.", "EXTS.", "FALL.", "FLD.", "FLDS.", "FLS.", "FLT.", "FLTS.", "FRD.", "FRDS.", "FRG.", "FRGS.", "FRK.", "FRKS.", "FRST.", "FRY.", "FT.", "FWY.", "GDN.", "GDNS.", "GLN.", "GLNS.", "GRN.", "GRNS.", "GRV.", "GRVS.", "GTWY.", "HBR.", "HBRS.", "HL.", "HLS.", "HOLW.", "HTS.", "HVN.", "HWY.", "I.", "INLT.", "IS.", "ISLE.", "ISS.", "JCT.", "JCTS.", "KNL.", "KNLS.", "KY.", "KYS.", "LAND.", "LCK.", "LCKS.", "LDG.", "LF.", "LGT.", "LGTS.", "LK.", "LKS.", "LN.", "LNDG.", "LOOP.", "MALL.", "MDW.", "MDWS.", "MEWS.", "MHD.", "ML.", "MLS.", "MNR.", "MNRS.", "MSN.", "MT.", "MTN.", "MTNS.", "MTWY.", "NCK.", "OPAS.", "ORCH.", "OVAL.", "PARK.", "PARK.", "PASS.", "PATH.", "PIKE.", "PKWY.", "PKWY.", "PL.", "PLN.", "PLNS.", "PLZ.", "PNE.", "PNES.", "PR.", "PRT.", "PRTS.", "PSGE.", "PT.", "PTS.", "RADL.", "RAMP.", "RD.", "RDG.", "RDGS.", "RDS.", "RIV.", "RNCH.", "ROW.", "RPD.", "RPDS.", "RST.", "RTE.", "RUE.", "RUN.", "SHL.", "SHLS.", "SHR.", "SHRS.", "SKWY.", "SMT.", "SPG.", "SPGS.", "SPUR.", "SQ.", "SQS.", "ST.", "STA.", "STRM.", "STS.", "TER.", "TPKE.", "TRAK.", "TRCE.", "TRL.", "TRWY.", "TUNL.", "UN.", "UNS.", "UPAS.", "VIA.", "VIS.", "VL.", "VLG.", "VLGS.", "VLY.", "VLYS.", "VW.", "VWS.", "WALK.", "WALK.", "WALL.", "WAY.", "WAYS.", "WL.", "WLS.", "XING.", "XRD." };

        private const string JSON_DATE_FORMAT = "yyyy-MM-ddTHH\\:mm\\:ss.fffZ";
        private const string DATE_ONLY_FORMAT = "yyyy/MM/dd";
        private const string TIME_ONLY_FORMAT = "HH\\:mm";

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static byte[] ToUTF8ByteArray(this string value)
        {
            return System.Text.UTF8Encoding.UTF8.GetBytes(value);
        }

        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }

        public static DateTime? ToNullableDateTime(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                DateTime result;
                if (DateTime.TryParseExact(value, JSON_DATE_FORMAT, new CultureInfo("en-US"), DateTimeStyles.None, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        
        public static string ToNuvIoTKey(this string value)
        {
            var regEx = new Regex("([0-9a-zA-Z]+)");
            var match = regEx.Match(value);

            var key = String.Empty;

            if (!match.Success)
            {
                return Guid.NewGuid().ToId();
            }

            while (match.Success)
            {
                key += match.Groups[1].Value.ToLower();
                match = match.NextMatch();
            }

            switch (key[0])
            {
                case '0': key = "z" + key; break;
                case '1': key = "o" + key; break;
                case '2': key = "t" + key; break;
                case '3': key = "th" + key; break;
                case '4': key = "f" + key; break;
                case '5': key = "fi" + key; break;
                case '6': key = "si" + key; break;
                case '7': key = "se" + key; break;
                case '8': key = "e" + key; break;
                case '9': key = "n" + key; break;
            }

            return key;
        }


        private static string NormalizeFormatString(String value)
        {
            value = value.TrimEnd('Z');

            if (value.Length == 19)
            {
                value = $"{value}.000Z";
            }
            else if (value.Length == 20)
            {
                value = $"{value}000Z";
            }
            else if (value.Length == 21)
            {
                value = $"{value}00Z";
            }
            else if (value.Length == 22)
            {
                value = $"{value}0Z";
            }
            else if (value.Length == 23)
            {
                value = $"{value}Z";
            }
            else
            {
                value = $"{value.Substring(0, 23)}Z";
            }

            return value;
        }

        public static bool SuccessfulJSONDate(this string date)
        {
            if (!date.EndsWith("Z"))
            {
                return false;
            }

            var dateValue = NormalizeFormatString(date);
            if (Regex.IsMatch(dateValue, @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.?\d{0,4}Z$"))
            {
                return DateTime.TryParseExact(dateValue, JSON_DATE_FORMAT, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime result);
            }
            else
            {
                return false;
            }
        }

        public static bool SuccessfulId(this string value)
        {
            return value.Length == 32;
        }

        public static bool IsValidId(this string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return false;
            }

            var regEx = new Regex("^[0-9A-F]{32}$");
            return regEx.Match(id).Success;
        }

        public static DateTime ToDateTime(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Must provide value to convert string to DateTime.");
            }

            /*
               Format = 
               10 Chars for Date
               1  for T
               8 for hh:mm:ss
               . for ms separator
               ms portion
               Z

               Min length w/o ms after trimming Z = 19
            */

            if (!value.EndsWith("Z"))
            {
                if (value.Length >= 8 && value.Length <= 10)
                {
                    if (DateTime.TryParse(value, out DateTime dateTimeValue))
                    {
                        return dateTimeValue;
                    }
                }
                else
                //HACK: - Maybe, sometimes the deserilaizer tries to help us by converting the ISO format into current date format, if so use that...not sure if this could be a sleeping bug.
                if (DateTime.TryParse(value, CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTimeValue))
                {
                    return dateTimeValue;
                }
                else
                {
                    throw new Exception($"Invalid Date Format, expected JSON ISO format in GMT ending with Z, value provided {value}");
                }
            }

            if (value.Length < 19)
            {
                throw new Exception($"Input Time Too Short, minimum is yyyy-mm-ddThh:mm:sssZ, value provided {value}");
            }

            var normalizedValue = NormalizeFormatString(value);

            if (DateTime.TryParseExact(normalizedValue, JSON_DATE_FORMAT, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime.ToUniversalTime();
            }

            throw new Exception($"Could Not Parse DateTime, value provided {value}.");
        }

        public static String ToJSONString(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToString(JSON_DATE_FORMAT);
            }
            else
            {
                return dateTime.ToUniversalTime().ToString(JSON_DATE_FORMAT);
            }
        }

        public static String ToDateOnly(this DateTime dateTime)
        {
            return dateTime.ToString(DATE_ONLY_FORMAT);
        }

        public static String ToDateOnly(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            else
            {
                return dateTime.Value.ToString(DATE_ONLY_FORMAT);
            }
        }

        public static String ToJSONString(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            else
            {
                return dateTime.Value.ToString(JSON_DATE_FORMAT);
            }
        }

        const char Base64Padding = '=';
        static readonly HashSet<char> base64Table = new HashSet<char>{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
                                                                      'P','Q','R','S','T','U','V','W','X','Y','Z','a','b','c','d',
                                                                      'e','f','g','h','i','j','k','l','m','n','o','p','q','r','s',
                                                                      't','u','v','w','x','y','z','0','1','2','3','4','5','6','7',
                                                                      '8','9','+','/' };

        public static bool IsBase64String(this string value)
        {
            value = value.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (value.Length == 0 || (value.Length % 4) != 0)
            {
                return false;
            }

            var lengthNoPadding = value.Length;
            value = value.TrimEnd(Base64Padding);
            var lengthPadding = value.Length;

            if ((lengthNoPadding - lengthPadding) > 2)
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!base64Table.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }

        public static string ToId(this Guid guid)
        {
            return guid.ToString().Replace("-", "").ToUpper();
        }

        public static string EmptyOrValue(this string value)
        {
            return value.IfEmpty(string.Empty);
        }

        public static string IfEmpty(this string value, string defaultValue)
        {
            return (!value.IsEmpty() ? value : defaultValue);
        }

        public static string FormatWith(this string value, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(value) || parameters == null || !parameters.Any()) { return value; }
            return value.FormatWith(CultureInfo.InvariantCulture, parameters);
        }

        public static string UpperTrim(this string value)
        {
            return (value.IsEmpty() ? string.Empty : value.Trim().ToUpper());
        }

        public static string CreateString(this byte[] array)
        {
            return System.Text.UTF8Encoding.UTF8.GetString(array, 0, array.Length);
        }

        //Convert HH:MM:SS into a TimeSpan
        public static TimeSpan? ToTimeSpan(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            if(value.Length == 4)
            {
                var hours = int.Parse(value.Substring(0, 2));
                var minutes = int.Parse(value.Substring(2, 2));
                var timeSpan = TimeSpan.Zero;
                timeSpan = timeSpan.Add(TimeSpan.FromMinutes(minutes));
                timeSpan = timeSpan.Add(TimeSpan.FromHours(hours));
                return timeSpan;
            }

            var valueParts = value.Split(':');
            if (valueParts.Length != 3)
                return null;

            try
            {
                var hours = Convert.ToInt32(valueParts[0]);
                var minutes = Convert.ToInt32(valueParts[1]);
                var seconds = Convert.ToInt32(valueParts[2]);
                var timeSpan = TimeSpan.FromSeconds(seconds);
                timeSpan = timeSpan.Add(TimeSpan.FromMinutes(minutes));
                timeSpan = timeSpan.Add(TimeSpan.FromHours(hours));

                return timeSpan;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ToRowKeyDatePart(this DateTime dateTime)
        {
            return (DateTime.MaxValue.Ticks - dateTime.Ticks).ToString("D19");
        }

        public static string ToInverseTicksRowKey(this DateTime dateTime)
        {
            return $"{dateTime.ToRowKeyDatePart()}.{Guid.NewGuid().ToId()}";
        }


        public static int? ToInt(this object basis, IFormatProvider cultureInfo)
        {
            if (basis == null) return null;
            var ci = cultureInfo ?? CultureInfo.InvariantCulture;

            int check = 0;
            return int.TryParse(basis.StringAndTrimValue(), NumberStyles.Any, ci, out check) ? (int?)check : null;
        }

        public static double? ToDouble(this object basis, IFormatProvider cultureInfo)
        {
            if (basis == null) return null;
            var ci = cultureInfo ?? CultureInfo.InvariantCulture;

            double check = 0;
            return Double.TryParse(basis.StringAndTrimValue(), NumberStyles.Any, ci, out check) ? (double?)check : null;
        }

        public static decimal? ToDecimal(this object basis, IFormatProvider cultureInfo)
        {
            if (basis == null) return null;
            var ci = cultureInfo ?? CultureInfo.InvariantCulture;

            decimal check = 0;
            return Decimal.TryParse(basis.StringAndTrimValue(), NumberStyles.Any, ci, out check) ? (decimal?)check : null;
        }

        public static float? ToFloat(this object basis, IFormatProvider cultureInfo)
        {
            if (basis == null) return null;
            var ci = cultureInfo ?? CultureInfo.InvariantCulture;

            float check = 0;
            return float.TryParse(basis.StringAndTrimValue(), NumberStyles.Any, ci, out check) ? (float?)check : null;
        }

        public static string CamelCase(this string basis)
        {
            return basis.Substring(0, 1).ToLower() + basis.Substring(1);
        }

        private static string StringAndTrimValue(this object value)
        {
            return value != null ? value.ToString().Trim() : string.Empty;
        }

        public static string FormatWith(this string basis, IFormatProvider cultureInfo, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(basis) || parameters == null || !parameters.Any()) { return basis; }
            var ci = cultureInfo ?? CultureInfo.InvariantCulture;

            if (!basis.Contains("{") || !basis.Contains("}")) { throw new ArgumentException(string.Format(ci, "The string to be formatted ({0}) is not formatted properly.", basis), "basis"); }
            return string.Format(ci, basis, parameters);
        }

        public static string StripCommonEndingPunctuation(this string basis)
        {
            var nada = string.Empty;
            return basis.Replace(",", nada).Replace(".", nada).Replace(";", nada).Replace(":", nada).Replace("?", nada).Replace("!", nada);
        }

        public static string[] ToNodeNameList(this string basis, bool removeSpaces = true)
        {
            switch (removeSpaces)
            {
                case false:
                    return basis.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                default:
                    return basis.Replace(@" ", string.Empty).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public static bool IsValidTime(this string time)
        {
            if (string.IsNullOrEmpty(time)) return false;
            if (time.Length != 5) return false;

            var regEx = new Regex(@"\d\d:\d\d");
            if (!regEx.Match(time).Success) return false;

            var part1 = time.Substring(0, 2);
            var hours = Convert.ToInt32(part1);
            if (hours < 0 || hours > 23) return false;

            var part2 = time.Substring(3, 2);
            var minutes = Convert.ToInt32(part2);
            if (minutes < 0 || minutes > 59) return false;

            return true;           
        }

        public static string GetCapitalLettersOnly(this string basis)
        {
            var result = new StringBuilder();
            foreach (var c in basis)
            {
                if (captials.Any(a => a == c)) result.Append(c);
            }
            return result.ToString();
        }

        public static string ShowIfNotEmpty(this string basis, string ignorePhrase = null, string format = null)
        {
            if (!string.IsNullOrWhiteSpace(ignorePhrase) && basis.StartsWith(ignorePhrase)) return string.Empty;
            return !string.IsNullOrWhiteSpace(basis) ? !string.IsNullOrWhiteSpace(format) ? format.FormatWith(basis) : basis : string.Empty;
        }

        public static string ToTitleCase(this string basis)
        {
            var result = new StringBuilder();
            foreach (var c in basis)
            {
                if (captials.Any(a => a == c))
                {
                    result.AppendFormat("{1}{0}", c, result.Length > 0 ? " " : string.Empty);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        public static bool IsEndOfSentence(this string basis)
        {
            var check = basis.ToUpperInvariant();
            return check.EndsWith(".") && !roadTypeAbbreviations.Any(a => check.EndsWith(a));
        }
    }

    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            Type enumType = value.GetType();

            /*            DescriptionAttribute[] loAttrib = (DescriptionAttribute[])(value.GetType().


							.GetField(value.ToString())
						   .GetCustomAttributes(typeof(DescriptionAttribute), false));
						return loAttrib.Length > 0 ? loAttrib[0].Description : value.ToString();*/

            return value.ToString();
        }

        public static T ToEnum<T>(this int value) where T : struct
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToEnum<T>(this string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T FromDescription<T>(this string value) where T : struct
        {
            foreach (T leEnumValue in Enum.GetValues(typeof(T)))
            {
                if (value == (leEnumValue as Enum).ToDescription())
                    return leEnumValue;
            }
            return default(T);
        }
    }

    public static class ListExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            var observeable = new ObservableCollection<T>();
            foreach (var item in items)
                observeable.Add(item);

            return observeable;
        }
    }

    public static class IIDExentiosn
    {
        public static void SetId(this IIDEntity entity)
        {
            entity.Id = Guid.NewGuid().ToId();
        }
    }

    public static class IAuditableExtensions
    {
        public static void SetCreationUpdatedFields(this IAuditableEntity entity, IEntityHeader user)
        {
            if (user == null || user.IsEmpty())
            {
                throw new InvalidOperationException("Must provide a valid user instance to assign to auditable fields.");
            }

            entity.CreationDate = DateTime.Now.ToJSONString();
            entity.CreatedBy = new EntityHeader()
            {
                Id = user.Id,
                Text = user.Text
            };

            entity.LastUpdatedDate = entity.CreationDate;
            entity.LastUpdatedBy = new EntityHeader()
            {
                Id = user.Id,
                Text = user.Text
            };
        }

        public static void SetCreationUpdatedFields(this ITableStorageAuditableEntity entity, IEntityHeader user)
        {
            if (user == null || user.IsEmpty())
            {
                throw new InvalidOperationException("Must provide a valid user instance to assign to auditable fields.");
            }

            entity.CreationDate = DateTime.Now.ToJSONString();
            entity.CreatedBy = user.Text;
            entity.CreatedById = user.Id;

            entity.LastUpdatedDate = entity.CreationDate;
            entity.LastUpdatedBy = user.Text;
            entity.LastUpdatedById = user.Id;
        }

        public static void SetLastUpdatedFields(this IAuditableEntity entity, IEntityHeader user)
        {
            entity.LastUpdatedDate = DateTime.Now.ToJSONString();
            entity.LastUpdatedBy = new EntityHeader()
            {
                Id = user.Id,
                Text = user.Text
            };
        }
    }
}

#region Alt

//38 43 33 19 
//0d 0a
//int lnEndIndex = Array.IndexOf(value, ISCPDefinitions.EndCharacter[Properties.Settings.Default.ISCP_EndMessage]);
//if (lnEndIndex < 0)
//    lnEndIndex = value.Length - Properties.Settings.Default.ISCP_HeaderSize;
//string lsReturnMessage = ASCIIEncoding.ASCII.GetString(value, Properties.Settings.Default.ISCP_HeaderSize, (lnEndIndex - Properties.Settings.Default.ISCP_HeaderSize)).TrimEnd();
//Logger.Debug("To Message {0}", lsReturnMessage);

//public static List<string> ToISCPStatusMessage(this byte[] value, out byte[] poNotProcessingBytes)
//{

//    return value.ToISCPStatusMessage(Properties.Settings.Default.ISCP_EndMessage, out poNotProcessingBytes);
//}

//public static List<string> ToISCPStatusMessage(this byte[] value, string psEndCharacter)
//{
//    if (value == null || value.Length == 0)
//        throw new ArgumentException("value is null or empty.", "value");
//    if (value.Length <= Properties.Settings.Default.ISCP_HeaderSize)
//        throw new ArgumentException("value is not an ISCP-Message.", "value");
//    const int lnDataSizePostion = 8;
//    const int lnDataSizeBytes = 4;
//    List<string> loReturnList = new List<string>();
//    string lsMessage;

//    foreach (int lnISCPIndex in SearchStartIndexHeader(value))
//    {
//        if (value.Length > (lnISCPIndex + lnDataSizePostion + 4))
//        {
//            int lnDataSize = BitConverter.ToInt32(Enumerable.Take(value.Skip(lnISCPIndex + lnDataSizePostion), lnDataSizeBytes).Reverse().ToArray(), 0);
//            if (value.Length >= (lnISCPIndex + Properties.Settings.Default.ISCP_HeaderSize + lnDataSize))
//            {
//                lsMessage = ConvertMessage(value, lnISCPIndex + Properties.Settings.Default.ISCP_HeaderSize, lnDataSize);
//                Logger.Debug("To Message {0}", lsMessage);
//                loReturnList.Add(lsMessage);
//            }
//        }
//    }


//    return loReturnList;
//}

#endregion

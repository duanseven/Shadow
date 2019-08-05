using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using NSun.Data;

namespace System
{
    /// <summary>
    /// The CommonUtils class.
    /// </summary>
    public static class CommonUtils
    {
        #region DefaultValue

        /// <summary>
        /// Gets the default value of a specified Type.
        /// </summary>
        /// <returns>The default value.</returns>
        public static T DefaultValue<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Gets the default value of a specified Type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object DefaultValue(Type type)
        {
            return typeof(CommonUtils).GetMethod("DefaultValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null).MakeGenericMethod(type).Invoke(null, null);
        }

        public static bool IsDefaultValue(Type type, object obj)
        {
            if (NSun.Data.Helper.NullableHelper.IsNullableType(type) || type == typeof(string) || type == typeof(byte[]))
            {
                if (ReferenceEquals(obj, null))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region GetType

        /// <summary>
        /// Gets a type in all loaded assemblies of current app domain.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        public static Type GetType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return null;
            }
            Type t = null;
            if (fullName.StartsWith("System.Nullable`1["))
            {
                string genericTypeStr = fullName.Substring("System.Nullable`1[".Length).Trim('[', ']');
                if (genericTypeStr.Contains(","))
                {
                    genericTypeStr = genericTypeStr.Substring(0, genericTypeStr.IndexOf(",")).Trim();
                }
                return typeof(Nullable<>).MakeGenericType(GetType(genericTypeStr));
            }
            try
            {
                t = Type.GetType(fullName);
            }
            catch
            {
            }

            if (t == null)
            {
                try
                {
                    if (fullName.Contains(","))
                    {
                        string[] classNameAssembly = fullName.Split(',');
                        Assembly ass = Assembly.LoadFrom(classNameAssembly[1]);
                        if (ass != null)
                            t = ass.GetType(classNameAssembly[0]);
                    }
                    else
                    {
                        Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();

                        for (int i = asses.Length - 1; i >= 0; i--)
                        {
                            Assembly ass = asses[i];
                            try
                            {
                                t = ass.GetType(fullName);
                            }
                            catch
                            {
                            }

                            if (t != null)
                            {
                                break;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return t;
        }

        /// <summary>
        /// GetOriginalTypeOfNullableType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetOriginalTypeOfNullableType(Type type)
        {
            if (type.ToString().StartsWith("System.Nullable`1["))
            {
                return GetType(type.ToString().Substring("System.Nullable`1[".Length).Trim('[', ']'));
            }

            return type;
        }

        #endregion

        #region String Utils

        private static Random rnd = new Random();


        public static string MakeGuidUniqueKey(string prefix)
        {
            var guidstr = Guid.NewGuid().ToString("N");//32
            var str = string.Concat(prefix, guidstr);
            return str;
        }

        /// <summary>
        /// Makes a unique key.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        public static string MakeUniqueKey(int length, string prefix)
        {
            int prefixLength = prefix == null ? 0 : prefix.Length;
            string chars = "1234567890abcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            if (prefixLength > 0)
            {
                sb.Append(prefix);
            }
            int dupCount = 0;
            int preIndex = 0;
            for (int i = 0; i < length - prefixLength; ++i)
            {
                int index = rnd.Next(0, 35);
                if (index == preIndex)
                {
                    ++dupCount;
                }
                sb.Append(chars[index]);
                preIndex = index;
            }
            if (dupCount >= length - prefixLength - 2)
            {
                rnd = new Random();
                return MakeUniqueKey(length, prefix);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Makes a unique key.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string MakeUniqueKey(int length)
        {
            return MakeUniqueKey(length, null);
        }

        /// <summary>
        /// Replace the first occurrence of <paramref name="find"/> (case sensitive) with
        /// <paramref name="replace"/>.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="find">The find.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public static string ReplaceFirst(string str, string find, string replace)
        {
            return ReplaceFirst(str, find, replace, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Replace the first occurrence of <paramref name="find"/> with
        /// <paramref name="replace"/>.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="find">The find.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="findComparison">The find comparison.</param>
        /// <returns></returns>
        public static string ReplaceFirst(string str, string find, string replace, StringComparison findComparison)
        {

            int firstIndex = str.IndexOf(find, findComparison);
            if (firstIndex != -1)
            {
                if (firstIndex == 0)
                {
                    str = replace + str.Substring(find.Length);
                }
                else if (firstIndex == (str.Length - find.Length))
                {
                    str = str.Substring(0, firstIndex) + replace;
                }
                else
                {
                    str = str.Substring(0, firstIndex) + replace + str.Substring(firstIndex + find.Length);
                }
            }
            return str;
        }

        #endregion

        #region File & Path Helpers

        /// <summary>
        /// Parses the relative path to absolute path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public static string ParseRelativePath(string basePath, string relativePath)
        {
            if (relativePath.StartsWith("\\") || relativePath.StartsWith(".\\") || relativePath.Contains(":"))
            {
                return System.IO.Path.GetFullPath(relativePath);
            }

            basePath = basePath.Trim().Replace("/", "\\");
            relativePath = relativePath.Trim().Replace("/", "\\");

            string[] splittedBasePath = basePath.Split('\\');
            string[] splittedRelativePath = relativePath.Split('\\');

            StringBuilder sb = new StringBuilder();
            int parentTokenCount = 0;
            for (int i = 0; i < splittedRelativePath.Length; i++)
            {
                if (splittedRelativePath[i] == "..")
                {
                    parentTokenCount++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < splittedBasePath.Length - parentTokenCount; i++)
            {
                if (!string.IsNullOrEmpty(splittedBasePath[i]))
                {
                    sb.Append(splittedBasePath[i]);
                    sb.Append("\\");
                }
            }

            for (int i = parentTokenCount; i < splittedRelativePath.Length; i++)
            {
                if (!string.IsNullOrEmpty(splittedRelativePath[i]))
                {
                    sb.Append(splittedRelativePath[i]);
                    sb.Append("\\");
                }
            }

            return sb.ToString().TrimEnd('\\');
        }

        /// <summary>
        /// Combines the two paths, making sure no two slashes are combined.
        /// </summary>
        /// <param name="path1">The path1.</param>
        /// <param name="path2">The path2.</param>
        /// <returns></returns>
        public static string PathCombine(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path2))
            {
                return path1;
            }

            if (path2[0] == '\\' || path2[0] == '/')
            {
                path2 = path2.Substring(1);
            }

            return Path.Combine(path1, path2);
        }

        /// <summary>
        /// Gets the temporary directory.
        /// </summary>
        /// <returns></returns>
        public static string GetTemporaryDirectory()
        {
            string ret = PathCombine(Path.GetTempPath(), "t" + new Random((int)unchecked(DateTime.Now.Ticks)).Next(1, 10000).ToString() + @"\");
            Directory.CreateDirectory(ret);
            return ret;
        }

        /// <summary>
        /// Gets the location of a new temporary file name with the given
        /// extension. Extension should not begin with a period (e.g. just html, not .html).
        /// The file is created on disk with a file size of 0. It is guaranteed
        /// that the file is a new file that did not exist before.
        /// </summary>
        /// <param name="extension">The preferred file extension. Extension should not begin with a period (e.g. just html, not .html).</param>
        /// <returns>Location of the 0-byte file in a temporary location with the specified extension.</returns>
        public static string GetTempFileName(string extension)
        {
            return GetTempFileName(extension, null);
        }

        /// <summary>
        /// Gets the location of a new temporary file name with the given file name and
        /// extension. Extension should not begin with a period (e.g. just html, not .html).
        /// File name should not end with a period and should not contain the extension
        /// (as that is in the extension parameter).
        /// The file is created on disk with a file size of 0. It is guaranteed
        /// that the file is a new file that did not exist before.
        /// </summary>
        /// <param name="extension">The preferred file extension. Extension should not begin with a period (e.g. just html, not .html).</param>
        /// <param name="fileName">The preferred name of the file, without a trailing period, and without an extension (as that is specified by the extension parameter).</param>
        /// <returns>Location of the 0-byte file in a temporary location with the specified extension and name.</returns>
        public static string GetTempFileName(string extension, string fileName)
        {
            string tempFile = Path.GetTempFileName();

            // Find the last slash
            int lastSlash = tempFile.LastIndexOf('\\');

            string filePart = tempFile.Substring(lastSlash + 1);

            tempFile = tempFile.Substring(0, lastSlash + 1);

            // Split the file name into extension and name
            int periodIndex = filePart.LastIndexOf('.');

            string preName = null;

            if (periodIndex != -1)
            {
                preName = filePart.Substring(0, periodIndex);
            }

            if (fileName == null)
            {
                if (preName == null)
                {
                    tempFile += "t" + new Random((int)unchecked(DateTime.Now.Ticks)).Next(1, 10000).ToString();
                }
                else
                {
                    tempFile += preName;
                }
            }
            else
            {
                tempFile += fileName;
            }

            tempFile += "." + extension;

            return tempFile;
        }

        /// <summary>
        /// Saves the input stream to file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="newFile">The new file.</param>
        public static void SaveStreamToFile(Stream stream, string newFile)
        {
            // Now, write out the file
            int length = 256;
            byte[] buffer = new byte[length];
            using (FileStream fs = new FileStream(newFile, FileMode.Create))
            {
                int bytesRead = stream.Read(buffer, 0, length);
                // write the required bytes
                while (bytesRead > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    bytesRead = stream.Read(buffer, 0, length);
                }
            }
        }

        #endregion

        #region Misc

        /// <summary>
        /// Check equal of two arrays
        /// </summary>
        /// <param name="leftArr">left array</param>
        /// <param name="rightArr">right array</param>
        /// <returns></returns>
        public static bool IsArrayEquals(IEnumerable leftArr, IEnumerable rightArr)
        {
            if ((leftArr == null && rightArr != null) ||
                (leftArr != null && rightArr == null))
            {
                return false;
            }

            IEnumerator enLeft = leftArr.GetEnumerator();
            IEnumerator enRight = rightArr.GetEnumerator();

            return IsArrayEquals(enLeft, enRight);
        }

        /// <summary>
        /// Check equal of two arrays
        /// </summary>
        /// <param name="enLeft">left array</param>
        /// <param name="enRight">right array</param>
        /// <returns></returns>
        public static bool IsArrayEquals(IEnumerator enLeft, IEnumerator enRight)
        {
            if ((enLeft == null && enRight != null) ||
                (enLeft != null && enRight == null))
            {
                return false;
            }

            bool isEquals = true;

            while (true)
            {
                bool leftHasNext = enLeft.MoveNext();
                bool rightHasNext = enRight.MoveNext();

                if ((leftHasNext && (!rightHasNext)) ||
                    ((!leftHasNext) && rightHasNext))
                {
                    isEquals = false;
                    break;
                }

                if ((!leftHasNext) && (!rightHasNext))
                {
                    break;
                }

                if ((enLeft.Current == null && enRight.Current != null) ||
                    (enLeft.Current != null && enRight.Current == null) ||
                    (!enLeft.Current.Equals(enRight.Current)))
                {
                    isEquals = false;
                    break;
                }
            }

            return isEquals;
        }

        /// <summary>
        /// Convert enumerator to List
        /// </summary>
        /// <param name="en">the enumerator</param>
        /// <returns></returns>
        public static List<ReturnElementType> ToObjectList<ReturnElementType>(IEnumerator en)
        {
            List<ReturnElementType> list = new List<ReturnElementType>();
            while (en.MoveNext())
            {
                list.Add((ReturnElementType)en.Current);
            }
            return list;
        }

        public static List<T> ConvertArrayToList<T>(T[] arr)
        {
            List<T> list = new List<T>();
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; ++i)
                    list.Add(arr[i]);
            }

            return list;
        }

        public static bool IsEqual(object left, object right)
        {
            return IsEqual(left, right, false);
        }

        public static bool IsEqual(object left, object right, bool treatNullAndEmptyStringAsEqual)
        {
            if (left == null && right == null)
                return true;

            bool retValue = (left != null && left.Equals(right));
            if ((!retValue) && treatNullAndEmptyStringAsEqual)
            {
                if (left != null)
                    retValue = (left.ToString() == string.Empty && right == null);
                else retValue = (right.ToString() == string.Empty);
            }
            return retValue;
        }

        internal static object GetKeyValue(object obj, string colname)
        {
            return (from pinfo in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance)
                    where pinfo.Name.ToLower() == colname.Split('.')[1].ToLower()
                    select pinfo.GetValue(obj, null)).FirstOrDefault();
        }

        #endregion

        #region PageSize
        /// <summary>
        /// 分页算页数 
        /// First Page currentpage is 1
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentpage"></param>
        /// <returns></returns>
        public static int GetSkipCount(int pagesize, int currentpage)
        {
            return (currentpage - 1) * pagesize;
        }

        /// <summary>
        /// 得到当前页
        /// First Page currentpage is 1
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="skipcount"></param>
        /// <returns></returns>
        public static int GetCurrentIndex(int pagesize, int skipcount)
        {
            return (skipcount / pagesize) + 1;
        }

        #endregion

        #region No-Public Methods

        public static IQueryTable GetThisQueryTable<TTable>() where TTable : IQueryTable
        {
            return Activator.CreateInstance<TTable>();
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.IO;

namespace WXFramwork.Resource
{
    public class UpdateBundleDataInfo
    {
        /// <summary>
        /// 是否需要更新
        /// </summary>
        public bool NeedUpdate = false;
        
        /// <summary>
        /// 客户端更新时间
        /// </summary>
        internal string UpdateTime = "";
        
        /// <summary>
        /// 需要更新的总大小
        /// </summary>
        public long NeedUpdateSize = 0;
        
        /// <summary>
        /// CRC信息字典，key 为文件名，value 为 crc32检索码
        /// </summary>
        internal readonly Dictionary<string, Dictionary<string, uint>> PackageCRCDic = new Dictionary<string, Dictionary<string, uint>>();
        
        /// <summary>
        /// CRC对应的写入流
        /// </summary>
        internal readonly Dictionary<string, StreamWriter> PackageCRCFileDic = new Dictionary<string, StreamWriter>();
        
        /// <summary>
        /// 版本号， int[本地版本，远端版本]
        /// </summary>
        internal readonly Dictionary<string, int[]> PackageToVersion = new Dictionary<string, int[]>();

    }
}
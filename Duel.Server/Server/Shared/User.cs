using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Shared
{
    /// <summary>
    /// 공유된 사용자 정보. Db에 저장되고 로딩됨 
    /// </summary>
    public class User
    {
        public string Account { get; set; }

        public string Nick { get; set; }

        public string Password { get; set; }

        public string DeviceId { get; set; }

        public int Gold { get; set; }
    }
}

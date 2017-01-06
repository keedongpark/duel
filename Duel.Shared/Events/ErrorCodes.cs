using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public enum ErrorCodes
    {
        Success = 0,

        LoginBegin = 0,
        FailLogin, 
        FailLogin_UserNotExist,
        FailLogin_DuplicateLogin,
        FailLogin_PasswordIncorrect,
        LoginEnd,

        DbBegin = LoginBegin + 1000,
        FailDb_Op,                          /* General database error */
        FailDb_NotExist,                    /* Requested key not exist */
        DbEnd,

        InstanceBegin = DbBegin + 1000,
        FailInstance_MemberNotFound,
        InstanceEnd,


        GameBegin = InstanceBegin + 1000, 
        GameEnd,
    }
}

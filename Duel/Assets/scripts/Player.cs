using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts
{
    /// <summary>
    /// 게임 플레이어 상위 클래스.
    /// </summary>
    public class Player
    {
       

        public virtual void Finish()
        {

        }

        protected virtual void OnGameStart(string cmd)
        {
        }
    }
}

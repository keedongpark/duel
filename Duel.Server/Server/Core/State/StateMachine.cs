using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Assets.scripts
{
    /// <summary>
    /// Transition 처리 담당.
    /// </summary>
    public class StateMachine<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 상태 전환 옵션들
        /// </summary>
        public enum TransitionOptions
        {
            Safe,
            Immediate
        }

        /// <summary>
        /// 상태 전환 정보
        /// </summary>
        struct Transition
        {
            public TKey Target;
            public TransitionOptions option;
        }

        private State<TKey> current;
        private State<TKey> root;
        private Queue<Transition> tranQ;
        private bool transiting;

        /// <summary>
        /// 현재 활성화된 상태. Initialize 후 root 상태
        /// </summary>
        public State<TKey> Current
        {
            get { return current; }
        }

        /// <summary>
        /// 루트 상태
        /// </summary>
        public State<TKey> Root
        {
            get { return root; }
        }

        /// <summary>
        /// 생성자. Initialize를 통해 설정
        /// </summary>
        public StateMachine()
        {
            tranQ = new Queue<Transition>();
        }

        /// <summary>
        /// Initialize machine with a root state which has all children.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public StateMachine<TKey> Initialize(State<TKey> root)
        {
            this.root = root;
            this.current = this.root;

            this.root.Initialize(this);
            this.root.Enter(); 

            return this;
        }

        /// <summary>
        /// 상태 실행. 실행 전 트랜지션 처리.
        /// </summary>
        public void Execute()
        {
            // Safe 트랜지션들 확인 후 진행

            while (tranQ.Count > 0)
            {
                var tran = tranQ.Dequeue();

                TransitTo(tran.Target);
            }

            if ( current != null)
            {
                current.Execute();
            }
        }

        /// <summary>
        /// 트랜지션 진행. 다음 Execute에서 전환 
        /// </summary>
        /// <param name="TargetKey"></param>
        /// <param name="option"></param>
        public StateMachine<TKey> Transit(TKey TargetKey)
        {
            Transition tran;
            tran.Target = TargetKey;
            tran.option = TransitionOptions.Safe;

            tranQ.Enqueue(tran);

            return this;
        } 

        /// <summary>
        /// target 상태로 전환. 중간 상태들도 처리
        /// </summary>
        /// <param name="target"></param>
        private void TransitTo(TKey target)
        {
            Debug.Assert(current != null);
            Debug.Assert(current.IsActive);

            if (current.Equals(target))
            {
                // 같은 상태로는 전환되지 않도록 한다. 
                return;
            }

            List<State<TKey>.TransitionNode> path = new List<State<TKey>.TransitionNode>();

            // Target State 까지 패스를 만들어 리스트를 가져온다.
            var rc = current.GetTransitionPathTo(target, path);

            if (!rc)
            {
                return;
            }

            foreach (var n in path)
            {
                // 패스 목록을 돌면서 상태를 Exit 하고 Enter를 한다. Target이 최종목표이고 noop는 Root다
                switch (n.code)
                {
                    case State<TKey>.TransitionNode.Code.Enter:
                        {
                            n.state.Enter();
                        }
                        break;
                    case State<TKey>.TransitionNode.Code.Target:
                        {
                            n.state.Enter();

                            current = n.state;
                        }
                        break;
                    case State<TKey>.TransitionNode.Code.TargetNoop:
                        {
                            current = n.state;
                        }
                        break;
                    case State<TKey>.TransitionNode.Code.Exit:
                        {
                            n.state.Exit();
                        }
                        break;
                    case State<TKey>.TransitionNode.Code.Noop:
                        {
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Assets.scripts
{

    /// <summary>
    /// 정적인 구성을 갖는 계층적 상태 기계.
    /// </summary>
    /// <typeparam name="TKey">상태 구분자</typeparam>
    public abstract class State<TKey> where TKey : IEquatable<TKey>
    {
        #region 타잎과 변수들
 
        /// <summary>
        /// 상태 전환을 위한 동작 값 갖는 노드
        /// </summary>
        public struct TransitionNode
        {
            public enum Code : byte
            {
                Noop = 1      // nothing to do
                , Exit          // Exit the state
                , Enter         // Enter the state
                , Target        // Target state
                , TargetNoop    // Target이 이미 실행 중일 때
            };

            public Code code;
            public State<TKey> state;
        }

        /// <summary>
        /// 간단한 디버깅용 통계 정보. overflow시 0으로 초기화
        /// </summary>
        public struct Stats
        {
            public uint enterCount;
            public uint exitCount;
            public uint executionCount;
        }

        private StateMachine<TKey> machine;
        private Dictionary<TKey, State<TKey>> childs;
        private TKey key;
        private bool isActive;
        private State<TKey> activeChild;
        private Stats stats;
        #endregion 

        public TKey Key
        {
            get { return key; }
        }

        /// <summary>
        /// 자식에서 직접 찾음
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public State<TKey> this[TKey key]
        {
            get
            {
                return childs[key];
            }
        }

        /// <summary>
        /// 부모 상태
        /// </summary>
        public State<TKey> Parent
        {
            get; set;
        }

        /// <summary>
        /// 부모 갖고 있는 지 여부
        /// </summary>
        public bool HasParent
        {
            get { return Parent != null; }
        }

        /// <summary>
        /// 최상위 상태 
        /// </summary>
        public State<TKey> Root
        {
            get
            {
                State<TKey> current = this;
                State<TKey> root = current;

                while (current != null)
                {
                    root = current;
                    current = Parent;
                }

                return root;
            }
        }

        /// <summary>
        /// 활성화 여부 
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
        }

        /// <summary>
        /// 자식들 중 현재 활성화된 상태 가져오기
        /// </summary>
        public State<TKey> ActiveChild
        {
            get { return activeChild; }
        }

        public StateMachine<TKey> Machine
        {
            get { return machine; }
        }

        public Stats Stat
        {
            get { return stats; }
        }

        /// <summary>
        /// 생성자 
        /// </summary>
        /// <param name="key"></param>
        public State(TKey key)
        {
            this.key = key;
            this.childs = new Dictionary<TKey, State<TKey>>();
            this.isActive = false;
        }

        /// <summary>
        /// StateMachine 에서 호출해서 machine 정보를 전달한다.
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        public State<TKey> Initialize(StateMachine<TKey> machine)
        {
            this.machine = machine;

            foreach (var child in childs.Values)
            {
                child.Initialize(machine);
            }

            return this;
        }

        /// <summary>
        /// 나를 포함해서 key 상태가 있는 지 확인
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Has(TKey key)
        {
            return (Find(key) != null);
        }

        /// <summary>
        /// 나의 1세대 자손이 key를 갖고 있는 지 확인
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasChild(TKey key)
        {
            return childs[key] != null;
        }

        /// <summary>
        /// 나를 제외하고 자손 중에 key를 갖고 있는 지 확인
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasDecendent(TKey key)
        {
            foreach (var child in childs.Values)
            {
                if (child.Has(key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add child states
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        public State<TKey> Add(params State<TKey>[] states)
        {
            for (int i = 0; i < states.Length; ++i)
            {
                Debug.Assert(
                    Root.Find(states[i].Key) == null, 
                    "State Key must be unique in a machine"
                );

                states[i].Parent = this;
                childs.Add(states[i].Key, states[i]);
            }

            return this;
        }

        public StateMachine<TKey> Transit(TKey target)
        {
            Debug.Assert(machine != null);

            machine.Transit(target);

            return machine;
        }

        /// <summary>
        /// 나와 자식들 중에 key를 갖는 상태 찾기
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public State<TKey> Find(TKey key)
        {
            if (this.key.Equals(key))
            {
                return this;
            }

            foreach (var child in childs.Values)
            {
                var s = child.Find(key);    

                if ( s != null )
                {
                    return s;
                }
            }

            return null;
        }

        /// <summary>
        /// 상태 진입 시 처리 
        /// </summary>
        public void Enter()
        {
            isActive = true;

            if (HasParent)
            {
                Debug.Assert(Parent.IsActive);
                Debug.Assert(Parent.activeChild != this);

                Parent.activeChild = this;
            }

            stats.enterCount++;

            OnEnter();
        }

        /// <summary>
        /// 상태 실행 처리
        /// </summary>
        public void Execute()
        {
            Debug.Assert(isActive);

            OnExecute();

            if (activeChild != null)
            {
                activeChild.Execute();
            }

            stats.executionCount++;
        }

        /// <summary>
        /// 상태 나가기 처리
        /// </summary>
        public void Exit()
        {
            isActive = false;

            if (HasParent)
            {
                if (Parent.activeChild == this)
                {
                    Parent.activeChild = null;
                }
            }

            OnExit();

            stats.exitCount++;
        }

    

        /// <summary>
        /// 하위 클래스 진입 시 처리 
        /// </summary>
        protected virtual void OnEnter() { }
        
        /// <summary>
        /// 하위 클래스 실행 처리
        /// </summary>
        protected virtual void OnExecute() { }

        /// <summary>
        /// 하위 클래스 나갈 때 처리
        /// </summary>
        protected virtual void OnExit() { }


        /// <summary>
        /// 목표 상태로 가는 전환 경로 가져오기. StateMachine 내부 함수
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool GetTransitionPathTo(TKey target, List<TransitionNode> path)
        {
            if (Has(target))
            {
                if (target.Equals(Key))
                {
                    TransitionNode n;

                    n.state = this;

                    if (IsActive)
                    {
                        n.code = TransitionNode.Code.TargetNoop;
                    }
                    else
                    {
                        n.code = TransitionNode.Code.Target;
                    }

                    path.Add(n);

                    return true;
                }
                else
                {
                    TransitionNode n;

                    n.state = this;

                    if (IsActive)
                    {
                        n.code = TransitionNode.Code.Noop;
                    }
                    else
                    {
                        n.code = TransitionNode.Code.Enter;
                    }

                    path.Add(n);

                    return GetChildPathTo(target, path);
                }
            }
            else
            {
                if (Parent != null)
                {
                    TransitionNode n;

                    n.state = this;
                    n.code = TransitionNode.Code.Exit;

                    path.Add(n);

                    return Parent.GetTransitionPathTo(target, path);
                }
            }

            return false;
        }

        /// <summary>
        /// 자식 부터 시작하는 전이 경로 찾기 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool GetChildPathTo(TKey target, List<TransitionNode> path)
        {
            foreach (var child in childs.Values)
            {
                if (child.Has(target))
                {
                    return child.GetTransitionPathTo(target, path);
                }
            }

            return false; // No child is in the path
        }
    }
}

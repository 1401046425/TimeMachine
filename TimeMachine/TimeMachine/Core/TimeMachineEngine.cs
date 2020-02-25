using System;
using System.Collections.Generic;
/*
 * 时间机器引擎核心
 * 管理游戏中的时间机器
 * 实现时光倒流回溯功能
 */
namespace TimeMachine.Core
{
    public class TimeMachineEngine
    {
        public TimeMachineEngine()
        {
            _timeMachineList = new List<ITimeMachine>();
            _rewindActions = new Stack<RewindAction>();
        }
        #region Singleton (单例)

        private static TimeMachineEngine _instance;

        public static TimeMachineEngine Instance
        {
            get
            {
                if (null == _instance) _instance = new TimeMachineEngine();

                return _instance;
            }
        }

        #endregion

        #region Data (数据存储模块)

        private readonly Stack<RewindAction> _rewindActions; //回溯动作
        private readonly List<ITimeMachine> _timeMachineList; //时间机器

        #endregion

        #region Property (属性模块)

        private int _frameCount; //当前帧序号
        private bool _isRewindTime; //是否在设置时间
        private float _time; //时间
        public float DeltaTime { get; private set; }

        #endregion

        #region Core核心模块

        /// <summary>
        /// Time核心时间轴
        /// </summary>
        public float Time
        {
            get { return _time; }
            set
            {
                if (Math.Abs(value - _time) > 0)
                {
                    DeltaTime = value - _time;

                    //if (RewindEffect) GrayScaleImageEffect.Instance.IsRewindTime = deltaTime < 0;//如果有回溯效果则开启或关闭回溯效果

                    if (DeltaTime < 0)
                        while (_rewindActions.Count > 0 && _rewindActions.Peek().m_time >= _time + DeltaTime)
                        {
                            var curDeltaTime = _rewindActions.Peek().m_time - _time;

                            UpdateTime(curDeltaTime);

                            _rewindActions.Pop().m_action();
                            DeltaTime -= curDeltaTime;
                        }

                    UpdateTime(DeltaTime); //更新时间
                    _time = value;
                }
            }
        }

        /// <summary>
        /// FrameCount核心帧序
        /// </summary>
        public int FrameCount
        {
            get { return _frameCount; }
            set
            {
                _frameCount = value < 0 ? 1 : value;
                if (IsRewindTime) SetFrame(_frameCount); //如果在设置时间则设置帧
            }
        }

        /// <summary>
        /// 是否在设置时间
        /// </summary>
        public bool IsRewindTime
        {
            get { return _isRewindTime; }
            set
            {
                _isRewindTime = value;
                if (!value) EndSetFrame(_frameCount); //如果设置时间结束，则结束设置帧序
            }
        }

        #endregion

        #region Method方法

        /// <summary>
        /// 注册时间机器
        /// </summary>
        /// <param name="timeMachine">时间机器</param>
        public void RegisterTimeMachine(ITimeMachine timeMachine)
        {
            _timeMachineList.Add(timeMachine);
        }

        /// <summary>
        /// 注销时间机器
        /// </summary>
        /// <param name="timeMachine">时间机器</param>
        public void UnregisterTimeMachine(ITimeMachine timeMachine)
        {
            _timeMachineList.Remove(timeMachine);
        }

        /// <summary>
        /// 添加回溯动作
        /// </summary>
        /// <param name="action">动作</param>
        public void AddRewindAction(Action action)
        {
            _rewindActions.Push(new RewindAction(Time, action));
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        /// <param name="deltaTime">deltatime</param>
        private void UpdateTime(float deltaTime)
        {
            foreach (var timeMachine in _timeMachineList) timeMachine.UpdateTime(deltaTime);
        }

        /// <summary>
        /// 设置帧
        /// </summary>
        /// <param name="frame">帧序</param>
        private void SetFrame(int frame)
        {
            foreach (var timeMachine in _timeMachineList) timeMachine.FrameSet(frame);
        }

        /// <summary>
        /// 设置帧结束
        /// </summary>
        /// <param name="frame">帧序</param>
        private void EndSetFrame(int frame)
        {
            foreach (var timeMachine in _timeMachineList) timeMachine.EndFrameSet(frame);
        }

        #endregion
    }
}
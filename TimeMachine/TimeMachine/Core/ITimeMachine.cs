
namespace TimeMachine.Core
{
    public interface ITimeMachine
    {
        /// <summary>
        /// 更新时间
        /// </summary>
        /// <param name="deltaTime"></param>
        void UpdateTime(float deltaTime);
        /// <summary>
        /// 设置帧
        /// </summary>
        /// <param name="frameCount"></param>
        void FrameSet(int frameCount);
        /// <summary>
        /// 设置帧结束
        /// </summary>
        /// <param name="frameCount"></param>
        void EndFrameSet(int frameCount);
    }
}
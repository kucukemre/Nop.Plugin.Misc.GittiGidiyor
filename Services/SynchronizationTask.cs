using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.GittiGidiyor.Services
{
    /// <summary>
    /// Represents a schedule task to synchronize contacts
    /// </summary>
    public class SynchronizationTask : IScheduleTask
    {
        #region Fields

        private readonly GittiGidiyorManager _gittigidiyorManager;

        #endregion

        #region Ctor

        public SynchronizationTask(GittiGidiyorManager gittigidiyorManager)
        {
            _gittigidiyorManager = gittigidiyorManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            _gittigidiyorManager.Synchronize();
        }

        #endregion
    }
}
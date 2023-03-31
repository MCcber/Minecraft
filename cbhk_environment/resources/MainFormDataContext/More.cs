using cbhk_environment.More;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cbhk_environment.resources.MainFormDataContext
{
    public class More:ObservableObject
    {
        /// <summary>
        /// 进群交流
        /// </summary>
        public RelayCommand ConversationGroupCommand { set; get; }

        /// <summary>
        /// 反馈bug
        /// </summary>
        public RelayCommand FeedBackBugsCommand { get; set; }

        /// <summary>
        /// 关于编辑器
        /// </summary>
        public RelayCommand AboutUsCommand { get; set; }

        public More()
        {
            ConversationGroupCommand = new RelayCommand(conversation_group_command);
            FeedBackBugsCommand = new RelayCommand(feedback_bugs_command);
            AboutUsCommand = new RelayCommand(about_us_command);
        }

        /// <summary>
        /// 进群交流
        /// </summary>
        private void conversation_group_command()
        {
            ConversationGroup conversation_group = new ConversationGroup();
            conversation_group.Show();
        }

        /// <summary>
        /// 反馈bug
        /// </summary>
        private void feedback_bugs_command()
        {
            FeedBackBugs feedback_bugs = new FeedBackBugs();
            feedback_bugs.Show();
        }

        /// <summary>
        /// 关于编辑器
        /// </summary>
        private void about_us_command()
        {
            AboutUs about_us = new AboutUs();
            about_us.Show();
        }
    }
}

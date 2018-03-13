using Microsoft.Practices.Unity;
using XJTUWebApi.Controllers;

namespace XJTUWebApi.Ioc
{
    public class ControllerRegister
    {
        private IUnityContainer container;

        public ControllerRegister(IUnityContainer container)
        {
            this.container = container;
        }
        /// <summary>
        /// 注册控制器
        /// </summary>
        public void Regist()
        {
            container.RegisterType<TaskController>();
        }
    }
}

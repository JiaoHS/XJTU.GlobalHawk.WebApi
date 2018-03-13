using Microsoft.Practices.Unity;
using WebApi.Controllers;

namespace WebApi.Ioc
{
    public class ControllerRegister
    {
        private IUnityContainer container;

        public ControllerRegister(IUnityContainer container)
        {
            this.container = container;
        }

        public void Regist()
        {
            container.RegisterType<TestController>();
        }
    }
}

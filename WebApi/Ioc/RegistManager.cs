using Microsoft.Practices.Unity;
using WebApi.Ioc;

namespace WebApi.Ioc
{
    public class RegistManager
    {
        public static void RegistAll(IUnityContainer container)
        {
            new ControllerRegister(container).Regist();
            new ServiceRegister(container).Regist();
            new DaoRegister(container).Regist();
        }
    }
}

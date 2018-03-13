using Microsoft.Practices.Unity;

namespace XJTUWebApi.Ioc
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

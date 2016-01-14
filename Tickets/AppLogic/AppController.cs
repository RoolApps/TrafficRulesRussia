using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLogic
{
    public class AppController
    {
        #region Private Members
        private static AppController Controller { get; set; }
        public TestController TestController { get; private set; }
        #endregion

        #region Public Methods
        public static void InitApp()
        {
            if(Controller == null)
            {
                Controller = new AppController();
                Controller.InitAppPrivate();
            }
            else
            {
                throw new Exception("InitApp method should be called only once");
            }
        }

        public static StartTestInfo StartTest(TestParams parameters)
        {
            return Controller.StartTestPrivate(parameters);
        }
        #endregion

        #region Private Methods
        private void InitAppPrivate()
        {
            throw new NotImplementedException();
        }

        public StartTestInfo StartTestPrivate(TestParams parameters)
        {
            TestController controller = null;
            try
            {
                controller = new TestController(parameters);
                var validationResult = controller.ValidateParameters();
                switch(validationResult)
                {
                    case ParamsValidationResult.Valid:
                        TestController = controller;
                        controller.StartTest();
                        return new StartTestInfo() { Result = StartTestResult.Started };
                    case ParamsValidationResult.NoTickets:
                        return new StartTestInfo() { ErrorMessage = "Билеты не были выбраны", Result = StartTestResult.LogicError };
                    case ParamsValidationResult.DuplicateTickets:
                        return new StartTestInfo() { ErrorMessage = "Были переданы повторяющиеся номера билетов", Result = StartTestResult.LogicError };
                    default:
                        throw new NotImplementedException(String.Format("Not implemented StartTestResult handler: {0}", validationResult));
                }
            }
            catch(Exception ex)
            {
                return new StartTestInfo() { ErrorMessage = String.Format("Необработанное исключение: {0}", ex.Message), Result = StartTestResult.UnexpectedError };
            }
        }
        #endregion
    }

    #region AdditionalClasses
    public class StartTestInfo
    {
        public String ErrorMessage { get; set; }
        public StartTestResult Result { get; set; }
    }

    public enum StartTestResult
    {
        Started,
        LogicError,
        UnexpectedError
    }
    #endregion
}

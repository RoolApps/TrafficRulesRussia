﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Utils;

// Шаблон пустого приложения задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234227

namespace Tickets
{
    /// <summary>
    /// Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Инициализирует одноэлементный объект приложения. Это первая выполняемая строка разрабатываемого
        /// кода; поэтому она является логическим эквивалентом main() или WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            InitApp();
        }

        /// <summary>
        /// Подготавливает приложение к работе
        /// </summary>
        private void InitApp()
        {
            InitDBFile();
#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#endif
        }

        /// <summary>
        /// Убеждается, что файл файл базы данных находится в папке LocalFolder
        /// </summary>
        private void InitDBFile()
        {
            CheckDBFile().ContinueWith(async (result) =>
            {
                bool actual = await result;
                if (!actual)
                {
                    await CopyDBFile();
                }
            }).ContinueWith((task) =>
            {
                AppLogic.Static.PreloadedContent.LoadData();
            });
        }

        private async Task<bool> CheckDBFile()
        {
            try
            {
                var currentFile = await ApplicationData.Current.LocalFolder.GetFileAsync(AppData.Resources.DBFileName);
                var actualFile = await Package.Current.InstalledLocation.GetFileAsync(AppData.Resources.DBFileName);
                var provider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
                var currentFileBuffer = await FileIO.ReadBufferAsync(currentFile);
                var actualFileBuffer = await FileIO.ReadBufferAsync(actualFile);
                return provider.HashData(currentFileBuffer).ToArray().SequenceEqual(provider.HashData(actualFileBuffer).ToArray());
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private async Task<bool> CopyDBFile()
        {
            try
            {
                
                var dbFile = await Package.Current.InstalledLocation.GetFileAsync(AppData.Resources.DBFileName);
                await dbFile.CopyAsync(ApplicationData.Current.LocalFolder, AppData.Resources.DBFileName, NameCollisionOption.ReplaceExisting);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

#if WINDOWS_PHONE_APP
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.CanGoBack && rootFrame.CurrentSourcePageType != typeof(Tickets.MainPage))
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
            else
            {
                e.Handled = false;
            }
        }
#endif

        /// <summary>
        /// Вызывается при обычном запуске приложения пользователем.  Будут использоваться другие точки входа,
        /// если приложение запускается для открытия конкретного файла, отображения
        /// результатов поиска и т. д.
        /// </summary>
        /// <param name="e">Сведения о запросе и обработке запуска.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            
            Frame rootFrame = Window.Current.Content as Frame;

            // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
            // только обеспечьте активность окна
            if (rootFrame == null)
            {
                // Создание фрейма, который станет контекстом навигации, и переход к первой странице
                rootFrame = new Frame();

                // TODO: Измените это значение на размер кэша, подходящий для вашего приложения
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Загрузить состояние из ранее приостановленного приложения
                    string navigationState = await SettingSaver.GetSettingFromFile("NavigationState");
                    if(!String.IsNullOrEmpty(navigationState))
                    {
                        rootFrame.SetNavigationState(navigationState);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(MainPage));
                    }
                }

                // Размещение фрейма в текущем окне
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Удаляет турникетную навигацию для запуска.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // Если стек навигации не восстанавливается для перехода к первой странице,
                // настройка новой страницы путем передачи необходимой информации в качестве параметра
                // навигации
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Обеспечение активности текущего окна
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Восстанавливает переходы содержимого после запуска приложения.
        /// </summary>
        /// <param name="sender">Объект, где присоединен обработчик.</param>
        /// <param name="e">Сведения о событии перехода.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Вызывается при приостановке выполнения приложения.  Состояние приложения сохраняется
        /// без учета информации о том, будет ли оно завершено или возобновлено с неизменным
        /// содержимым памяти.
        /// </summary>
        /// <param name="sender">Источник запроса приостановки.</param>
        /// <param name="e">Сведения о запросе приостановки.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Сохранить состояние приложения и остановить все фоновые операции
            try
            {
                await SettingSaver.SaveSettingToFile("NavigationState", (Window.Current.Content as Frame).GetNavigationState());
            }
            catch (Exception)
            { }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
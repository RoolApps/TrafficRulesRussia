using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Utils
{
    public static class UIElementProperties
    {
        // Метод копирования(клонирования) любого UIElement, а также его потомков
        public static T Clone<T>(T source_object) where T : UIElement
        {
            T copy_object = default(T);
            if (source_object != null) {
                copy_object = Activator.CreateInstance(source_object.GetType()) as T;
                IEnumerable<PropertyInfo> properties = source_object.GetType().GetRuntimeProperties();
                foreach (PropertyInfo p in properties) {
                    if (p.Name != "Name") {
                        if (p.CanWrite && p.CanRead) {
                            // Вытаскиваем значение каждого св-ва из source_object
                            object obj = p.GetValue(source_object);
                            // Проставляем значение каждого св-ва в copy_object
                            try {
                                p.SetValue(copy_object, obj);
                            }
                            catch (Exception e) {
                                if (e.InnerException != null) {
                                    string err = e.InnerException.Message;
                                }
                            }
                        }
                    }
                }
                
                copy_object.SetValue(Canvas.LeftProperty, source_object.GetValue(Canvas.LeftProperty));
                copy_object.SetValue(Canvas.TopProperty, source_object.GetValue(Canvas.TopProperty));

                // Если этот UIelement производный от Panel (есть ли у него дочерние элементы)
                Panel source_panel = source_object as Panel;
                if (source_panel != null) {
                    if (source_panel.Children.Count != 0) {
                        Panel copy_panel = copy_object as Panel;
                        foreach (UIElement e in source_panel.Children) {
                            UIElement clone = UIElementProperties.Clone(e);
                            copy_panel.Children.Add(clone);
                        }
                    }
                }
            }
            return (T)copy_object;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHelper.Helpers
{
    public interface IToastWindow
    {
        public static IToastWindow BuildToastWindow(ToastMode toastMode = ToastMode.GHelper)
        {
            return toastMode switch
            {
                ToastMode.GHelper => new ToastForm(),
                ToastMode.Windows => new ToastWindowsStyle(),
                _ => throw new NotImplementedException()
            };
        }
        void RunToast(string text, ToastIcon? icon = null);
        void RunToast(string text, ToastIcon? icon = null, double? maxValue = null, double? value = null);
    }

    public enum ToastMode
    {
        GHelper = 0,
        Windows = 1,
        [Obsolete("unsupported")]
        ASUA = 2,
    }
}

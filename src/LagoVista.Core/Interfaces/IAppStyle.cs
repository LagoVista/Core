using LagoVista.Core.Models.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IAppStyle
    {
        Color TitleBarBackground { get; }
        Color TitleBarText { get; }
        Color PageBackground { get; }
        Color PageText { get; }

        Color LabelText { get; }
        Color EditControlBackground { get; }
        Color EditControlText { get; }

        Color EditControlFrame { get;  }
        Color EditControlFrameFocus { get; }
        Color EditControlFrameInvalid { get; }

        Color MenuBarBackground { get;  }

        Color MenuBarForeground { get; }
        Color MenuBarBackgroundActive { get; }
        Color MenuBarForegroundActive { get; }

        Color ButtonBackground { get; }
        Color ButtonBorder { get; }
        Color ButtonForeground { get; }


        Color ButtonBackgroundActive { get; }
        Color ButtonBorderActive { get; }
        Color ButtonForegroundActive { get; }

        Color HighlightColor { get; }

        Color RowSeperatorColor { get; }
    }
}

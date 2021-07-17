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
            
        Color ListItemColor { get; }

        Color ButtonBackgroundActive { get; }
        Color ButtonBorderActive { get; }
        Color ButtonForegroundActive { get; }

        Color HighlightColor { get; }

        Color RowSeperatorColor { get; }

        Color TabForground { get; }
        Color TabForgroundActive { get; }
        Color TabBackgroundActive { get; }
        Color TabBackground { get; }

        Color TabBarBackground { get; }

        string HeaderFont { get; }
        string ContentFont { get; }
        string LabelFont { get; }
        string EntryFont { get; }
        string MenuFont { get; }
        string ListItemFont { get; }
        string TabBarFont { get; }

        double TabBarIconFontSize { get; }
        double TabBarFontSize { get; }
        double HeaderFontSize { get; }
        double LabelFontSize { get; }
        double EntryFontSize { get; }
        double ContentFontSize { get; }
        double MenuFontSize { get; }
        double ListItemFontSize { get; }
    }
}

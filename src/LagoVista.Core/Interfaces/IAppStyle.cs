using LagoVista.Core.Models.Drawing;

namespace LagoVista.Core.Interfaces
{
    public interface IAppStyle
    {
        Color TitleBarBackground { get; }
        Color TitleBarForeground { get; }
       
        Color PageBackground { get; }
        Color PageForeground { get; }
        string PageTextFont { get; }
        double PageTextFontSize { get; }

        Color HighlightColor { get; }


        Color HeaderColor { get; }
        double HeaderFontSize { get; }
        string HeaderFont { get; }


        Color SubHeaderColor { get; }
        double SubHeaderFontSize { get; }
        string SubHeaderFont { get; }


        Color LabelText { get; }
        string LabelFont { get; }
        double LabelFontSize { get; }


        Color EntryBackground { get; }
        Color EntryForeground { get; }
        Color EntryFrameColor { get;  }
        Color EntryFrameColorFocus { get; }
        Color EntryFrameColorFrameInvalid { get; }
        string EntryFont { get; }
        double EntryFontSize { get; }
        double EntryMargin { get; }


        Color MenuBarBackground { get;  }
        Color MenuBarForeground { get; }
        Color MenuBarBackgroundActive { get; }
        Color MenuBarForegroundActive { get; }
        double MenuFontSize { get; }
        string MenuFont { get; }


        Color ButtonBackground { get; }
        Color ButtonBackgroundActive { get; }
        Color ButtonBorder { get; }
        Color ButtonBorderActive { get; }
        Color ButtonForeground { get; }
        Color ButtonForegroundActive { get; }

        Color IconButtonForeground { get; }
        Color IconButtonForegroundAcive { get; }
        double IconButtonFontSize { get; }


        string ListItemFont { get; }
        string ListItemDetailFont { get; }
        Color ListItemBackgroundColor { get; }
        Color ListItemForegroundColor { get; }
        double ListItemFontSize { get; }
        double ListItemDetailFontSize { get; }
        Color ListItemDetailForegroundColor { get; }
        Color RowSeperatorColor { get; }
       

        Color TabForground { get; }
        Color TabForgroundActive { get; }
        Color TabBackgroundActive { get; }
        Color TabBackground { get; }
        Color TabBarBackground { get; }
        double TabBarIconFontSize { get; }
        double TabBarFontSize { get; }
        string TabBarFont { get; }
    }
}

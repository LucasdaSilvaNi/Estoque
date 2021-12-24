using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sam.Web.Controles
{
    public partial class Calendario : System.Web.UI.UserControl
    {
        #region Regular expressions
        private static Regex regPrevMonth = new Regex(
        @"(?<PrevMonth><a.*?>&lt;</a>)",
        RegexOptions.IgnoreCase
        | RegexOptions.Singleline
        | RegexOptions.CultureInvariant
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );

        private static Regex regNextMonth = new Regex(
        @"(?<NextMonth><a.*?>&gt;</a>)",
        RegexOptions.IgnoreCase
        | RegexOptions.Singleline
        | RegexOptions.CultureInvariant
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            // turn user control to html code
            string output = Calendario.RenderToString(calDate);

            MatchEvaluator mevm = new MatchEvaluator(AppendMonth);
            output = regPrevMonth.Replace(output, mevm);

            MatchEvaluator mevb = new MatchEvaluator(AppendYear);
            output = regNextMonth.Replace(output, mevb);
            // output the modified code
            writer.Write(output);
        }

        /// <summary>
        /// The date displayed on the popup calendar
        /// </summary>
        public DateTime? SelectedDate
        {
            get
            {
                // null date stored or not set
                if (ViewState["SelectedDate"] == null)
                {
                    return null;
                }
                return (DateTime)ViewState["SelectedDate"];
            }
            set
            {
                ViewState["SelectedDate"] = value;
                if (value != null)
                {
                    calDate.SelectedDate = (DateTime)value;
                    calDate.VisibleDate = (DateTime)value;
                }
                else
                {
                    calDate.SelectedDate = new DateTime(0);
                    calDate.VisibleDate = DateTime.Now.Date;
                }
            }
        }

        private string AppendMonth(Match m)
        {
            return Calendario.RenderToString(Ddmonth) + "&nbsp";
        }

        private string AppendYear(Match m)
        {
            return "&nbsp" + Calendario.RenderToString(Ddyear);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var al = new ArrayList();

                for (var i = DateTime.Now.Year - 5; i <= DateTime.Now.Year + 10; i++)
                    al.Add(i);

                Ddyear.DataSource = al;
                Ddyear.SelectedValue = DateTime.Now.Year.ToString();
                Ddyear.DataBind();

                Ddmonth.SelectedValue = DateTime.Now.Month.GetHashCode().ToString("00");
            }
        }

        public static string RenderToString(Control c)
        {
            bool previousVisibility = c.Visible;
            c.Visible = true; // make visible if not

            // get html code for control
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter localWriter = new HtmlTextWriter(sw);
            c.RenderControl(localWriter);
            string output = sw.ToString();

            // restore visibility
            c.Visible = previousVisibility;

            return output;
        }

        protected void DdyearSelectedIndexChanged(object sender, EventArgs e)
        {
            calDate.VisibleDate = obterDataCalendario();
        }

        protected void DdmonthSelectedIndexChanged(object sender, EventArgs e)
        {
            calDate.VisibleDate = obterDataCalendario();
        }

        private DateTime obterDataCalendario()
        {
            return new DateTime(Int32.Parse(Ddyear.SelectedValue), Int32.Parse(Ddmonth.SelectedValue), calDate.SelectedDate.Day);
        }

        protected void calDate_DayRender(object sender, DayRenderEventArgs e)
        {
            HyperLink hlnk = new HyperLink();
            hlnk.Text = ((LiteralControl)e.Cell.Controls[0]).Text;
            hlnk.Attributes.Add("href", "javascript:SetDate('" +
            e.Day.Date.ToString("dd/MM/yyyy") + "');");
            e.Cell.Controls.Clear();
            e.Cell.Controls.Add(hlnk);
        }

        protected void calDate_SelectionChanged(object sender, EventArgs e)
        {
            Ddmonth.SelectedValue = calDate.SelectedDate.Month.ToString("00");
            Ddyear.SelectedValue = calDate.SelectedDate.Year.ToString();
        }
    }
}

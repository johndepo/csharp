using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TestProj
{
internal class CommonFunctions
    {
        //Constructor, do nothing
        internal CommonFunctions()
        {
        }

        #region Date Functions
        internal DateTime LastMonth_Start()
        {
            return LastMonth().AddDays((LastMonth().Day - 1) * -1);
        }

        internal DateTime LastMonth_End()
        {
            return LastMonth().AddDays((DateTime.DaysInMonth(LastMonth().Year, LastMonth().Month) - LastMonth().Day));
        }

        private DateTime LastMonth()
        {
            return System.DateTime.Now.AddMonths(-1);
        }
        #endregion

        #region Export Functions
        internal void ExportDataGridView(ref DataGridView dgv, string path)
        {
            StreamWriter sw = new StreamWriter(path);
            string header = String.Empty;
            int maxCol = dgv.Columns.Count;

            for (int i = 0; i < maxCol; i++)
            {
                header += dgv.Columns[i].Name + ",";
            }
            sw.WriteLine(header);

            foreach (DataGridViewRow dgvr in dgv.Rows)
            {
                foreach (DataGridViewCell dgvc in dgvr.Cells)
                {
                    sw.Write(dgvc.Value.ToString() + ",");
                }
                sw.Write('\n');
            }
            sw.Close();
        }
}
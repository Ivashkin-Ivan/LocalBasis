using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBasis.Model
{
    public class CustomLine
    {
        public Element Element { get; set; }
        public Line Line { get; set; }

        //Глобальная система координат это внутренее начало Revit

        //GlobalCoords
        public XYZ GlobalDirection { get; set; }
        public XYZ GlobalOrigin { get; set; }

        //Локальная система координат
        public Transform LocalCoordinateSystem { get; set; }

        //LocalCoords
        public XYZ LocalDirection { get; set; }
        public XYZ LocalOrigin { get; set; }


        public CustomLine(Element element)
        {
            Element = element;
            Line = (Element.Location as LocationCurve).Curve as Line;
        }
        public void SetNewGlobalCoords()
        {
            GlobalDirection = Line.Direction;
            GlobalOrigin = Line.Origin;
        }
        public void SetNewLocalCoords()
        {
            var line = MathCore.FromGlobalToLocal(LocalCoordinateSystem, Line); // мнимая линия, из которой нужно взять локальные координаты
            LocalDirection = line.Direction;
            LocalOrigin = line.Origin;
        }
    }
    
}

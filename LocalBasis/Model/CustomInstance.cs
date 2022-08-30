using Autodesk.Revit.DB;

namespace LocalBasis.Model
{
    public class CustomInstance
    {
        public Element Element { get; set; }
        public Instance Instance { get; set; }

        //Глобальная система координат это внутренее начало Revit
        
        //GlobalCoords
        public XYZ GlobalBasisX { get; set; }
        public XYZ GlobalBasisY { get; set; }
        public XYZ GlobalBasisZ { get; set; }
        public XYZ GlobalOrigin { get; set; }
        //Для вывода
        public string GlobalText = "привет";

        //Локальная система координат
        public Transform LocalCoordinateSystem { get; set; }

        //LocalCoords
        public XYZ LocalBasisX { get; set; }
        public XYZ LocalBasisY { get; set; }
        public XYZ LocalBasisZ { get; set; }
        public XYZ LocalOrigin { get; set; }
        //Для вывода
        public string LocalText;

        public CustomInstance(Element element)
        {
            Element = element;
            Instance = (Element as Instance);
            SetNewGlobalCoords();
            SetNewLocalCoords();
        }
        public void SetNewGlobalCoords()
        {
            var transform = Instance.GetTotalTransform();
            GlobalBasisX = transform.BasisX;
            GlobalBasisY = transform.BasisY;
            GlobalBasisZ = transform.BasisZ;
            GlobalOrigin = transform.Origin;
            if (GlobalText == "привет")
                GlobalText = "пока";
            if (GlobalText == "пока")
                GlobalText = "привет";

        }
        public void SetNewLocalCoords()
        {
            Transform inLocal = MathCore.FromGlobalToLocal(LocalCoordinateSystem, Instance); //в идеале вернуть instance, стоит это сделать через создание семейства
            LocalBasisX = inLocal.BasisX;
            LocalBasisY = inLocal.BasisY;
            LocalBasisZ = inLocal.BasisZ;
            LocalOrigin = inLocal.Origin;
        }
    }
}

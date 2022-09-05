using Autodesk.Revit.DB;
using LocalBasis.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LocalBasis.Model
{
    public class CustomInstance : INotifyPropertyChanged
    {
        public TrackCommand trackCommand { get; set; }
        public Element Element { get; set; }
        public Instance Instance { get; set; }

        //Глобальная система координат это внутренее начало Revit

        //GlobalCoords
        public XYZ GlobalBasisX { get; set; }
        public XYZ GlobalBasisY { get; set; }
        public XYZ GlobalBasisZ { get; set; }
        public XYZ GlobalOrigin { get; set; }
        //Для вывода
        private string globalText;
        public string GlobalText
        {
            get
            {
                return globalText;
            }
            set
            {
                globalText = value;
                OnPropertyChanged("GlobalText");
            }
        }
        public int i = 0;

        //Локальная система координат
        public Transform LocalCoordinateSystem { get; set; }

        //LocalCoords
        public XYZ LocalBasisX { get; set; }
        public XYZ LocalBasisY { get; set; }
        public XYZ LocalBasisZ { get; set; }
        public XYZ LocalOrigin { get; set; }
        //Для вывода

        public string LocalText;

        public event PropertyChangedEventHandler PropertyChanged;
        public CustomInstance(Element element, Transform localSystem)
        {
            Element = element;
            Instance = Element as Instance;
            LocalCoordinateSystem = localSystem;
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
        }
        public void SetNewLocalCoords()
        {
            var mc = new MathCore();
            Transform inLocal = mc.FromGlobalToLocal(LocalCoordinateSystem, Instance); //в идеале вернуть instance, стоит это сделать через создание семейства
            LocalBasisX = inLocal.BasisX;
            LocalBasisY = inLocal.BasisY;
            LocalBasisZ = inLocal.BasisZ;
            LocalOrigin = inLocal.Origin;
        }
        public void CreateNewTable()
        {
            SetNewGlobalCoords();
            SetNewLocalCoords();

            GlobalText = null;
            //Таблица в главной системе======================================
            GlobalText += "Координаты instance Revit"+"\r\n\n";

            GlobalText += "Матрица направляющих cos-ов" + "\r\n";
            GlobalText += GlobalBasisX.ToString() + "\r\n";
            GlobalText += GlobalBasisY.ToString() + "\r\n";
            GlobalText += GlobalBasisZ.ToString() + "\r\n\n";

            GlobalText += "Координаты точки размещения" + "\r\n";
            GlobalText += GlobalOrigin.ToString() + "\r\n";
            //===============================================================


            LocalText = null;
            //Таблица в локальной системе====================================
            LocalText += "Координаты instance Местная система" + "\r\n\n";

            LocalText += "Матрица направляющих cos-ов" + "\r\n";
            LocalText += LocalBasisX.ToString() + "\r\n";
            LocalText += LocalBasisY.ToString() + "\r\n";
            LocalText += LocalBasisZ.ToString() + "\r\n\n";

            LocalText += "Координаты точки размещения" + "\r\n";
            LocalText += LocalOrigin.ToString() + "\r\n";
            //==============================================================
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

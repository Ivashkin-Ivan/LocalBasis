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
        private string globalText = "привет";
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
            i++;
            GlobalText = i.ToString();
        }
        public void SetNewLocalCoords()
        {
            Transform inLocal = MathCore.FromGlobalToLocal(LocalCoordinateSystem, Instance); //в идеале вернуть instance, стоит это сделать через создание семейства
            LocalBasisX = inLocal.BasisX;
            LocalBasisY = inLocal.BasisY;
            LocalBasisZ = inLocal.BasisZ;
            LocalOrigin = inLocal.Origin;
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

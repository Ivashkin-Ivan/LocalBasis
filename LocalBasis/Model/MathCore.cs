using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBasis.Model
{
    public static class MathCore
    {
        public static Transform CreateLocalCoordinateSystem(Line foundation, Document document)
        {
            // Получим из отрезка начало(конец) и направляющий вектор
            XYZ vector = foundation.Direction;
            XYZ origin = foundation.Origin;
            FamilyInstance cube = new FilteredElementCollector(document) //Нужно создвать экземпляр куба => нужно добавить проверку isActivate
                                      .OfClass(typeof(FamilyInstance))
                                      .Cast<FamilyInstance>()
                                      .Last(it => it.Symbol.FamilyName == "RedCube" && it.Symbol.Name == "Красный");
           
            XYZ facingOrientation = cube.FacingOrientation;
            double angle;

            if (vector.X > 0)
            {
                angle = 2 * Math.PI - vector.AngleTo(facingOrientation); //Угол между направляющим вектором и FacingOrientation
            }
            else
            {
                angle = vector.AngleTo(facingOrientation); //Угол между направляющим вектором и FacingOrientation  
            }
            Line rotationAxis = Line.CreateBound(origin, origin + new XYZ(0, 0, 1));

            //==============================================================//Постановка cube положение новой системы координат
            (cube.Location as LocationPoint).Point = origin;
            (cube.Location as LocationPoint).Rotate(rotationAxis, angle);
            //==============================================================

            // Рассмотрим плоский xOy случай, повернём вокруг ориджина на угол взятый из метода AngleTo(),
            // Обобщ модель (типо система координат, примагничивается в трубе и поворачивается на нужный угол, при условии, что первоначальная
            // ориентация куба по базису doc

            //====================================================//Получение прямого и обратного преобразования
            Instance instance = cube as Instance;
            Transform transform = instance.GetTotalTransform();       // Матрица
            Transform inverse = instance.GetTotalTransform().Inverse; //Обратная матрица - сохраняю эту строку для понятности
                                                                      //====================================================
            document.Delete(cube.Id); //Система готова, куб больше не нужен

            return transform; //Хранит информацию о новой систме координат
        }
        public static Transform CreateLocalCoordinateSystem(Instance instance, Document document)
        {
            Transform transform = instance.GetTotalTransform(); //Из instance можно сразу получить систему координат
            return transform; //Хранит информацию о новой систме координат
        }
        public static Line FromGlobalToLocal(Transform localCoordinateSystem, Line foundation)
        {
            Transform inverse = localCoordinateSystem.Inverse; //Получение обратной матрицы

            XYZ direction = foundation.Direction; //Распаковка линии
            XYZ origin = foundation.Origin;       //Распаковка линии

            XYZ directionInLocalSystem = inverse.OfVector(direction);
            XYZ originInLocalSystem = inverse.OfPoint(origin);

            Line line = Line.CreateUnbound(originInLocalSystem, originInLocalSystem); //Запаковка линии

            return line;
        }
        public static Transform FromGlobalToLocal(Transform localCoordinateSystem, Instance instance) // Прийдётся возвращать трансформ
        {
            // Так как Revit матрица считается кватрнионной и работает в R^4, а размерность локального базиса R^3, то линейная комбинация последнего столбца GlobalOrigin работает некорректно
            // в произвдении матриц, приходится вручную брать метод OfPoint и переписывать данный столбец дополнитльным действием. В debug-е GlobalBasisX, GlobalBasisY, GlobalBasisZ
            // можно рассматривать как матрицу 3х3, а строку GlobalOrigin представлять как столбец приписанный к этой матрице справа, 4 строка заполняется нулями, в месте (4;4) стоит единица
            Transform inverse = localCoordinateSystem.Inverse; //Получение обратной матрицы

            Transform inGlobal = instance.GetTotalTransform(); //У instance нельзя создать экземпляр - это небольшая проблемка
            Transform inLocal = inGlobal * inverse;
            inLocal.Origin = inverse.OfPoint(inGlobal.Origin); //данное доп действие нужно делать вне метода
            return inLocal; //заглушка
        }
        public static Line FromLocalToGlobal(Transform localCoordinateSystem, Line foundation)
        {
            XYZ direction = foundation.Direction; //Распаковка линии
            XYZ origin = foundation.Origin;       //Распаковка линии

            XYZ directionInGlobalSystem = localCoordinateSystem.OfVector(direction);
            XYZ originInGlobalSystem = localCoordinateSystem.OfPoint(origin);

            Line line = Line.CreateUnbound(originInGlobalSystem, originInGlobalSystem); //Запаковка линии

            return line;
        }
        public static Instance FromLocalToGlobal(Transform localCoordinateSystem, Instance instance)
        {
            // Так как Revit матрица считается кватрнионной и работает в R^4, а размерность локального базиса R^3, то линейная комбинация последнего столбца GlobalOrigin работает некорректно
            // в произвдении матриц, приходится вручную брать метод OfPoint и переписывать данный столбец дополнитльным действием. В debug-е GlobalBasisX, GlobalBasisY, GlobalBasisZ
            // можно рассматривать как матрицу 3х3, а строку GlobalOrigin представлять как столбец приписанный к этой матрице справа, 4 строка заполняется нулями, в месте (4;4) стоит единица


            //Экземпляр instance всё та же проблемка




            return null; //заглушка
        }


    }
}

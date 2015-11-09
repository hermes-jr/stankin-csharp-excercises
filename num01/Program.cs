using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace conapp_test
{
	// Константы для удобства (вместо #define в Си)
	class Constants
	{
		// Для классов
		public const ushort MAX_RND = 50;

		// Для задачи
		public const ushort PARAM1 = 20;
		public const ushort MATRIX_ROWS = 14;
		public const ushort MATRIX_COLS = 5;
	}

	/* Класс базовой матрицы, при наследовании позволяет создать матрицу
	 * произвольного размера, заполненную превдослучайными целыми числами
	 * (подходит для большинства задач, в некоторых возможно нужно будет
	 * изменить тип на float/double)
	 */
	public class MatrixBasic
	{
		// Пара свойств, по условию задачи
		protected ushort Rows { get; set; }
		protected ushort Columns { get; set; }

		// Двумерный прямоугольный массив, в котором будут храниться элементы
		public double[,] Elements;

		// Индексатор, по условию задачи
		public double this[int row, int col]
		{
			get
			{
				return Elements[row, col];
			}
			set
			{
				Elements[row, col] = (double) value;
			}
		}

		// Конструктор класса по умолчанию
		public MatrixBasic()
		{
			this.Rows = 3;
			this.Columns = 2;
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		// Конструктор объекта
		public MatrixBasic(ushort rows, ushort cols)
		{
			this.Rows = (rows != 0) ? rows : (ushort) 2;
			this.Columns = (cols != 0) ? cols : (ushort) 2;
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		/* Заполнение матрицы псевдослучайными целыми числами (элементы
		 * в диапазоне от -MAX_RND до MAX_RND включительно). В зависимости от
		 * условия задачи тут могут быть изменены типы данных
		 */
		public void Populate()
		{
			Random random = new Random();
			/* Бежим по матрице слева направо сверху вниз (большинство циклов
			 * будут реализованы так)
			 */
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					// ~случайный выбор знака + или -
					int sign = (int) Math.Pow(-1, random.Next(2));
					// ~случайный выбор значения элемента и тут же присвоение знака
					int currentNumber = random.Next(Constants.MAX_RND) * sign;

					this.Elements[iterR, iterC] = currentNumber;
				}
			}
		}

		// Вывод матрицы в виде таблицы
		public void Print()
		{
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				System.Console.Write('|'); // Левая граница, для красоты
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					/* Форматирование: N+1 символов в ширину (максимум N-значное число
					 * и знак. Если надо - увеличить). Пробел справа от числа и правая
					 * граница столбца
					 */
					System.Console.Write(String.Format(" {0,5:0.##} |", Elements[iterR, iterC]));
				}

				// Строка матрицы закончилась, переходим на следующую
				System.Console.Write('\n');
			}
			System.Console.WriteLine();
		}
	}

	/* Класс, описывающий адаптированную для условия текущей задачи матрицу,
	 * например квадратную или треугольную. По условию задач данный класс
	 * унаследован от обычной матрицы
	 */
	public class MatrixEx:MatrixBasic
	{
		// Конструктор по умолчанию
		public MatrixEx()
		{
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		// Конструктор объекта
		public MatrixEx(ushort rows, ushort cols)
		{
			this.Rows = (rows != 0) ? rows : (ushort) 2;
			this.Columns = (cols != 0) ? cols : (ushort) 2;
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		// Относящиеся к задаче функции и вычисления

		public void MaxFirst()
		{
			// Индекс максимального элемента (т.е. его номер в столбце, с нуля считаем)
			int curMaxIndex;

			// Работаем со столбцами, поэтому прогон по строкам внутри
			for(int iterC = 0; iterC < this.Columns; iterC++)
			{
				curMaxIndex = 0;

				for(int iterR = 0; iterR < this.Rows; iterR++)
				{
					if(Elements[iterR, iterC] >= Elements[curMaxIndex, iterC])
					curMaxIndex = iterR;
				}
				// По столбцу пробежались, знаем место максимального элемента, меняем его местами с первым:

				if(curMaxIndex == 0)
					continue; // Ибо самого с собой элемент менять местами не надо

				double valMax = Elements[curMaxIndex, iterC];
				Elements[curMaxIndex, iterC] = Elements[0, iterC];
				Elements[0, iterC] = valMax;
			}
		}

		/* Проверка, если хоть один элемент первой строки по модулю меньше заданного числа,
		 * вертаем false, иначе true
		 */
		public bool FrontLineBigger(int excParam1)
		{
			for(int iterC = 0; iterC < this.Columns; iterC++)
			{
				if(Math.Abs(Elements[0, iterC]) < excParam1)
					return false;
			}
			return true;
		}

		// Разделить элементы последней строки на соответствующие элементы первой строки
		public void DivideLastByFirst()
		{
			for(int iterC = 0; iterC < this.Columns; iterC++)
			{
				// На ноль делить нельзя
				if(Elements[0, iterC] != 0)
					Elements[this.Rows - 1, iterC] = Elements[this.Rows - 1, iterC] / Elements[0, iterC];
			}
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			// Заданная по условию задачи величина (менять в коде, либо дописывать чтение ввода пользователя)
			int excParam1 = Constants.PARAM1;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			// Первое действие по условию задачи - в каждом столбце максимальный элемент на первое место
			me.MaxFirst();
			// И сразу проверяем
			me.Print();

			if(me.FrontLineBigger(excParam1))
			{
				// Крайнее условие задачи соблюдено, делим последнюю строку на первую:
				me.DivideLastByFirst();
				me.Print();
			}
			else
			{
				System.Console.WriteLine("Est' elementy po modulu menshe zadannogo chisla");
			}

			Console.ReadKey(); // Ждём выброс и валим нахер!
		}
	}
}

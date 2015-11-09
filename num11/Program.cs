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
		public const ushort MAX_RND = 6;

		// Для задачи
		public const ushort MATRIX_SIDE = 5;
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
		public int[,] Elements;

		// Индексатор, по условию задачи
		public int this[int row, int col]
		{
			get
			{
				return Elements[row, col];
			}
			set
			{
				Elements[row, col] = (int) value;
			}
		}

		// Конструктор класса по умолчанию
		public MatrixBasic()
		{
			this.Rows = 3;
			this.Columns = 2;
			Elements = new int[this.Rows, this.Columns];
			this.Populate();
		}

		// Конструктор объекта
		public MatrixBasic(ushort rows, ushort cols)
		{
			this.Rows = (rows != 0) ? rows : (ushort) 2;
			this.Columns = (cols != 0) ? cols : (ushort) 2;
			Elements = new int[this.Rows, this.Columns];
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
					System.Console.Write(String.Format(" {0,3} |", Elements[iterR, iterC]));
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
			Elements = new int[this.Rows, this.Columns];
			this.Populate();
		}

		// Конструктор объекта
		public MatrixEx(ushort side)
		{
			this.Rows = (side != 0) ? side : (ushort) 2;
			this.Columns = this.Rows;
			Elements = new int[this.Rows, this.Columns];
			this.Populate();
		}

		// Относящиеся к задаче функции и вычисления

		// Координаты максимального элемента
		public ushort[] GetMaxCoords()
		{
			double maxEl = this.Elements[0, 0];
			ushort[] coords = new ushort[2];

			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					if(this.Elements[iterR, iterC] >= maxEl)
					{
						maxEl = this.Elements[iterR, iterC];
						coords[0] = iterR;
						coords[1] = iterC;
					}
				}
			}

			return coords;
		}

		/* Транспонируем матрицу, столбцы становятся строками и наоборот.
		 * Неоптимальный способ с пожиранием огромного количества памяти
		 */
		public void Transp()
		{
			// Дублируем матрицу чтобы не терять значений
			int[,] transped = new int[this.Columns, this.Rows];

			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					transped[iterC, iterR] = this.Elements[iterR, iterC];
				}
			}

			this.Elements = transped;
		}

		// Считаем сумму элементов искомой строки
		public double CountRow(ushort row)
		{
			double ret = 0.00;

			for(ushort iterC = 0; iterC < this.Columns; iterC++)
				ret += this.Elements[row, iterC];

			return ret;
		}

		// Считаем сумму элементов искомого столбца
		public double CountCol(ushort col)
		{
			double ret = 0.00;

			for(ushort iterR = 0; iterR < this.Rows; iterR++)
				ret += this.Elements[iterR, col];

			return ret;
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			ushort[] maxElemCoords;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём квадратную матрицу, в конструктор передаём размер матрицы
			MatrixEx me = new MatrixEx(Constants.MATRIX_SIDE);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			maxElemCoords = me.GetMaxCoords();

			System.Console.WriteLine(String.Format("Max element v {0} stroke {1} stolbce", maxElemCoords[0] + 1, maxElemCoords[1] + 1));

			// У всех элементов выше главной диагонали индекс строки строго меньше индекса столбца
			if(maxElemCoords[0] < maxElemCoords[1])
			{
				// Выводим транспонированную матрицу
				me.Transp();
				me.Print();
			}
			else
			{
				// Показать количество элементов выше главной диагонали, отличных от нуля
				System.Console.WriteLine(String.Format("Summa stroki: {0,5:0.###}, stolbca: {1,5:0.###}", me.CountRow(maxElemCoords[0]), me.CountCol(maxElemCoords[1])));
			}

			Console.ReadKey();
		}
	}
}

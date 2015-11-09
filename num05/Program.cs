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
		public const ushort MATRIX_SIDE = 8;
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

			/* По условию задачи матрица может быть нижнетреугольной или просто квадратной,
			 * сгенерируем с вероятностью 0.5 нижнетреугольную матрицу для разнообразия
			 */
			if((new Random()).Next(2) % 2 == 0)
			{
				this.Triangulate();
			}
		}

		protected void Triangulate()
		{
			ushort iter = this.Columns;

			// Бежим по матрице, Rows = Cols, но тут для наглядности разные переменные
			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					if(iterC > this.Columns - iter)
					{
						this.Elements[iterR, iterC] = 0;
					}
				}
				iter--;
			}
		}

		// Относящиеся к задаче функции и вычисления

		// Считаем среднее арифметическое матрицы, всё тупо
		public double CalcW()
		{
			double sum = 0.00;
			double tot = 0.00;

			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					sum += this.Elements[iterR, iterC];
					tot++;
				}
			}

			// По идее в матрице не может быть 0 элементов, но на всякий случай проверка, ибо на ноль делить нельзя :)
			if(tot != 0)
				return sum/tot;
			else
				return 0.00;
		}

		// Посчитать сколько элементов выше главной диагонали отличны от нуля
		public int CountUpEls()
		{
			int ret = 0;
			ushort iter = this.Columns;

			// Бежим по матрице, Rows = Cols, но тут для наглядности разные переменные
			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					// Если элемент находится выше диагонали и при этом не равен 0, считаем его
					if((iterC > this.Columns - iter) && this.Elements[iterR, iterC] != 0)
					{
						ret++;
					}
				}
				iter--;
			}

			return ret;
		}

		// Пробегаемся по элементам выше главной диагонали, если хоть один не равен 0, то матрица не нижнетреугольная
		public bool IsTriangular()
		{
			ushort iter = this.Columns;

			// Бежим по матрице, Rows = Cols, но тут для наглядности разные переменные
			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					// Если элемент находится выше диагонали и при этом не равен 0, матрица не треугольная
					if((iterC > this.Columns - iter) && this.Elements[iterR, iterC] != 0)
					{
						return false;
					}
				}
				iter--;
			}

			return true;
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём квадратную матрицу, в конструктор передаём размер матрицы
			MatrixEx me = new MatrixEx(Constants.MATRIX_SIDE);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			if(me.IsTriangular())
			{
				// Выводим среднее арифметическое матрицы
				System.Console.WriteLine(String.Format("Sredneye arifmet: {0,6:0.####}", me.CalcW()));
			}
			else
			{
				// Показать количество элементов выше главной диагонали, отличных от нуля
				System.Console.WriteLine(String.Format("Vyshe glavnoy diagonaly {0} el-tov ne ravny 0", me.CountUpEls()));
			}

			Console.ReadKey();
		}
	}
}

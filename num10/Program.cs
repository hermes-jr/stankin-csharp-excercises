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
		public const ushort MAX_RND = 5;

		// Для задачи
		public const ushort MATRIX_ROWS = 10;
		public const ushort MATRIX_COLS = 4;
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

		// Считаем количество нулей во всей матрице
		public int CountNulls()
		{
			int nullz = 0;

			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					if(this.Elements[iterR, iterC] == 0)
						nullz++;
				}
			}

			return nullz;
		}

		// Проверяем условие задачи
		public bool IsAsc()
		{
			ushort[] nullzByCol = new ushort[this.Columns];

			// Пробегаемся по матрице, считаем нули
			for(int iterC = 0; iterC < this.Columns; iterC++)
			{
				for(int iterR = 0; iterR < this.Rows; iterR++)
				{
					if(this.Elements[iterR, iterC] == 0)
						nullzByCol[iterC]++;
				}
			}

			// Проверяем, расположены ли количества нулей по возрастанию
			for(int iter = 1; iter < this.Columns; iter++)
			{
				if(nullzByCol[iter] < nullzByCol[iter - 1])
					return false; // Встретился столбец в котором нулей меньше, чем в предыдущем
			}

			return true;
		}

		public int MaxNullsColumn()
		{
			int maxId = 0;
			ushort[] nullzByCol = new ushort[this.Columns];

			// Пробегаемся по матрице, считаем нули
			for(int iterC = 0; iterC < this.Columns; iterC++)
			{
				for(int iterR = 0; iterR < this.Rows; iterR++)
				{
					if(this.Elements[iterR, iterC] == 0)
						nullzByCol[iterC]++;
				}
			}

			for(int iter = 0; iter < this.Columns; iter++)
			{
				if(nullzByCol[iter] >= nullzByCol[maxId])
					maxId = iter;
			}

			return maxId;
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу, в конструктор передаём размер матрицы
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();
			if(me.IsAsc())
			{
				// Ищем сколько всего нулей, это условие выполняется редко, ~ 1 раз из 10
				System.Console.WriteLine("Kolvo nulevih elementov po vozrastaniu");
				System.Console.WriteLine(String.Format("Vsego nuley: {0}", me.CountNulls()));
			}
			else
			{
				// По условию определяем номер столбца в котором максимум нулей
				System.Console.WriteLine(String.Format("V {0} stolbce maximum nuley", (ushort) (me.MaxNullsColumn() + 1)));
			}

			Console.ReadKey();
		}
	}
}
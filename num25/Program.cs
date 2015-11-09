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
		public const ushort MAX_RND = 4;

		// Для задачи

		public const ushort MATRIX_ROWS = 10;
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

		// Усечение матрицы до определённого размера
		public void Truncate(ushort rows, ushort cols)
		{
			if(rows > 0 && rows <= this.Rows)
				this.Rows = rows; // Чтобы не усложнять, не будем удалять лишние элементы

			if(cols > 0 && cols <= this.Columns)
				this.Columns = cols; // Чтобы не усложнять, не будем удалять лишние элементы
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

		// 
		public void MoveMinNullzFirst()
		{
			// Массив в котором будут храниться количества нулей соответственно столбцам, для сравнения
			double[] sums = new double[this.Columns];
			ushort minnulzId = 0;
			ushort iter = 0;

			// Сперва ищем столбец с наименьшим (не нулевым) количеством нулей
			for(int iterC = 0; iterC < this.Columns; iterC++)
			{
				for(int iterR = 0; iterR < this.Rows; iterR++)
				{
					if(this.Elements[iterR, iterC] == 0)
						sums[iterC]++;
				}
			}

			iter = 0;
			foreach(double val in sums)
			{
				if(val > 0 && val < sums[minnulzId])
					minnulzId = iter;
				iter++;
			}

			System.Console.WriteLine(String.Format("V {0} stolbce menshe vsego nuley", minnulzId + 1));

			if(minnulzId == 0)
				return; // Этот столбец уже первый, валим

			// Теперь меняем этот столбец с нулевым (первым)
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				double temp = this.Elements[iterR, minnulzId];
				this.Elements[iterR, minnulzId] = this.Elements[iterR, 0];
				this.Elements[iterR, 0] = temp;
			}
		}

		// В первом столбце сдвигаем нули вниз
		public void DownNullz()
		{
			ushort lastFree = (ushort) (this.Rows - 1);
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				// Кривая медленная конструкция, надо придумывать правильный способ, но лень
				while(lastFree >= 0)
				{
					if(lastFree > 0 && this.Elements[lastFree, 0] == 0)
					{
						lastFree--; // Свободного места не нашлось, ищем дальше
					}
					else
						break; // Есть свободное место (ненулевой элемент), индекс lastFree вычислен
				}
				double temp = this.Elements[iterR, 0]; // Запоминаем текущий элемент для перестановки
				if(temp == 0 && iterR < lastFree) // Забыл как я до этого условия допёр, но оно работает )))
				{
					// Элемент столбца отрицателен, меняем его с последним доступным элементом
					this.Elements[iterR, 0] = this.Elements[lastFree, 0];
					this.Elements[lastFree, 0] = temp;
				}
			}
		}

	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			me.MoveMinNullzFirst();
			me.DownNullz();
			me.Print();

			Console.ReadKey();
		}
	}
}
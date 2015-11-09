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
		public const uint PARAM1 = 15; // Параметр задачи задаваемый вручную
		public const ushort MATRIX_ROWS = 5;
		public const ushort MATRIX_COLS = 3;
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

		public double GetMinPos()
		{
			double ret = 0;
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					if(this.Elements[iterR, iterC] > 0 && ret == 0)
						ret = this.Elements[iterR, iterC];
					else if(this.Elements[iterR, iterC] > 0 && this.Elements[iterR, iterC] < ret)
					{
						// Если положительный и меньше текущего минимального
						ret = this.Elements[iterR, iterC];
					}
				}
			}
			return ret;
		}

		public double GetMaxNeg()
		{
			double ret = 0.00;
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					if(this.Elements[iterR, iterC] < 0 && ret == 0)
						ret = this.Elements[iterR, iterC];
					if(this.Elements[iterR, iterC] < 0 && this.Elements[iterR, iterC] > ret)
					{
						// Если отрицательный и больше текущего максимального
						ret = this.Elements[iterR, iterC];
					}
				}
			}
			return ret;
		}

		// По условию задачи меняем отрицательные элементы (тут для простоты - вообще все) на их модули
		public void Absolute()
		{
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					this.Elements[iterR, iterC] = Math.Abs(this.Elements[iterR, iterC]);
				}
			}
		}

	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			uint exParam1 = Constants.PARAM1;
			double minPos;
			double maxNeg;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу, в конструктор передаём размер матрицы
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			minPos = me.GetMinPos();
			maxNeg = me.GetMaxNeg();
			System.Console.WriteLine(String.Format("Minimalny polozhit: {0,5:0.###}, maximalny otricat: {1,5:0.###}", minPos, maxNeg));

			// Проверяем условие задачи
			double diff = Math.Abs(Math.Abs(minPos) - Math.Abs(maxNeg));
			System.Console.WriteLine(String.Format("Elementy otlichautsa na {0,5:0.###}", diff));
			if(diff < exParam1)
			{
				/* Так как элементы по модулю отличаются меньше, чем на заданную величину, то меняем
				 * элементы матрицы на их модули, после чего выводим изменённую матрицу
				 */
				me.Absolute();
				me.Print();
			}
			else
			{
				System.Console.WriteLine("Elementy otlichautsa bolshe chem na zadannuy velichinu ili rovno na nee");
			}

			Console.ReadKey();
		}
	}
}

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
		public const ushort MAX_RND = 10;

		// Для задачи
		public const ushort MATRIX_ROWS = 6;
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
					int currentNumber = random.Next(Constants.MAX_RND);// * sign;

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

		// Определить номер строки с максимальной суммой элементов
		public ushort GetMaxLineNumber(double[] sums)
		{
			double max = sums[0];
			ushort ind = 0;
			ushort iter = 0;

			foreach(double cval in sums)
			{
				if(cval >= max)
				{
					max = cval;
					ind = iter;
				}
				iter++;
			}

			System.Console.WriteLine(String.Format("Max summa elementov v {0} stroke: {1,5:0.###}", ind + 1, max));
			return ind;
		}

		// Определить номер строки с минимальной суммой элементов
		public ushort GetMinLineNumber(double[] sums)
		{
			double min = sums[0];
			ushort ind = 0;
			ushort iter = 0;

			foreach(double cval in sums)
			{
				if(cval <= min)
				{
					min = cval;
					ind = iter;
				}
				iter++;
			}

			System.Console.WriteLine(String.Format("Min summa elementov v {0} stroke: {1,5:0.###}", ind + 1, min));
			return ind;
		}

		// Посчитать суммы элементов для строк
		public double[] GetElementsSums()
		{
			/* Массив, размер которого равен кол-ву строк матрицы, каждой строке
			 * соответствует величина, равная сумме всех элементов этой строки
			 * матрицы... Не придумал как кошернее сделать %)
			 */
			double[] ret = new double[this.Rows];

			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				ret[iterR] = 0.00;
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					ret[iterR] += Elements[iterR, iterC];
				}
			}

			return ret;
		}

		// Меняем матрицу как сказано в условии задачи
		public void SwapRows(ushort minId, ushort maxId)
		{
			// Сюда будем запоминать заменяемую строку
			double[] temp = new double[this.Columns];

			/* Если вдруг минимальная сумма оказывается в первой строке, то без этой замены
			 * получится так, что мы сначала пихаем максимальную сумму в первую строку, а потом
			 * тут же перетаскиваем в последнюю, в итоге минимальная остаётся вообще где была,
			 * на её месте мусор, а на месте максимальной - минимальная. Фигак :)
			 */
			if(minId == 0)
				minId = maxId;

			if(maxId != 0)
			{
				/* Запоминаем нулевую (первую) и тут же запихиваем в неё значения
				 * из строки с максимальной суммой эл-тов
				 */
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					temp[iterC] = this.Elements[0, iterC];
					this.Elements[0, iterC] = this.Elements[maxId, iterC];
					this.Elements[maxId, iterC] = temp[iterC];
				}
			}
			if(minId != (this.Rows - 1))
			{
				/* Запоминаем последнюю и тут же запихиваем в неё значения
				 * из строки с минимальной суммой эл-тов
				 */
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					temp[iterC] = this.Elements[this.Rows - 1, iterC];
					this.Elements[this.Rows - 1, iterC] = this.Elements[minId, iterC];
					this.Elements[minId, iterC] = temp[iterC];
				}
			}
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{

			double[] sums;
			ushort minRow;
			ushort maxRow;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			// Считаем суммы элементов по строкам матрицы и запоминаем результат - пригодится
			sums = me.GetElementsSums();

			maxRow = me.GetMaxLineNumber(sums);

			minRow = me.GetMinLineNumber(sums);

			me.SwapRows(minRow, maxRow);
			me.Print();

			Console.ReadKey();
		}
	}
}

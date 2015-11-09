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

		public const ushort MATRIX_ROWS = 8;
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

		/* Функция, которая опускает все отрицательные значения вниз матрицы по столбцам.
		 * Принцип работы примерно такой: функция идёт по матрице сверху вниз слева направо,
		 * то есть сначала по первому столбцу сверху до низу, потом по второму и так далее.
		 * Для каждого элемента, если он отрицателен, ищется последнее доступное место снизу
		 * столбца (цикл while, индекс доступного элемента - lastFree), после чего отрицательный
		 * элемент меняется местами с этим самым последним неотрицательным. Лол, типа вот.
		 */
		public void DownNegatives()
		{
			for(int iterC = 0; iterC < this.Columns; iterC++)
			{
				ushort lastFree = (ushort) (this.Rows - 1);
				for(int iterR = 0; iterR < this.Rows; iterR++)
				{
					// Кривая медленная конструкция, надо придумывать правильный способ, но лень
					while(lastFree >= 0)
					{
						if(lastFree > 0 && this.Elements[lastFree, iterC] < 0)
						{
							lastFree--; // Свободного места не нашлось, ищем дальше
						}
						else
							break; // Есть свободное место (неотрицательный элемент), индекс lastFree вычислен
					}
					double temp = this.Elements[iterR, iterC]; // Запоминаем текущий элемент для перестановки
					if(temp < 0 && iterR < lastFree) // Забыл как я до этого условия допёр, но оно работает )))
					{
						// Элемент столбца отрицателен, меняем его с последним доступным элементом
						this.Elements[iterR, iterC] = this.Elements[lastFree, iterC];
						this.Elements[lastFree, iterC] = temp;
					}
				}
			}
		}

		// Функция, возвращающая индекс строки, на которой заканчиваются неотрицательные эл-ты
		public ushort GetN()
		{
			// Последний неотрицательный элемент находится в этой строке
			ushort firstNegIndex = this.Rows;

			// Пробегаем матрицу зигзагом в поисках первого отрицательного элемента
			for(ushort iterC = 0; iterC < this.Columns; iterC++)
			{
				for(ushort iterR = 0; iterR < firstNegIndex; iterR++)
				{
					if(this.Elements[iterR, iterC] < 0)
					{
						firstNegIndex = iterR;
						break;
					}
				}
			}

			return firstNegIndex;
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

			me.DownNegatives();
			me.Print();

			ushort eN = me.GetN();
			if(eN > 0)
			{
				System.Console.WriteLine(String.Format("Do N-noy stroki vkluchitelno otricatelnyh elementov net. N = {0}", eN));
				// Отрезаем матрицу снизу, метод унаследован из класса базовой матрицы
				me.Truncate(eN, 0);
				me.Print();
			}
			else
				System.Console.WriteLine("Odin ili neskolko stolbcov sostoyat tolko iz otricatelnyh el-tov");

			Console.ReadKey();
		}
	}
}

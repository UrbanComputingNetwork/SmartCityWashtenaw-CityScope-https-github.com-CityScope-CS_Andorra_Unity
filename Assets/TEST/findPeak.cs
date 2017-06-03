


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class findPeak : MonoBehaviour
{


	//* Write a program that finds the largest area of equal neighbor
	//elements in a rectangular matrix and prints its size. Example:
	//1     3       2       2       2       4
	//3     3       3       2       4       4
	//4     3       1       2       3       3
	//4     3       1       3       3       1
	//4     3       3       3       1       1
	void Start ()
	{ 

		int[,] numbers = {
			{ 1, 3, 2, 2, 2, 4, 9, 9, 9, 2, 4 },
			{ 3, 3, 3, 2, 4, 4, 9, 3, 9, 4, 4 },
			{ 4, 3, 1, 2, 3, 3, 9, 1, 9, 3, 3 },
			{ 4, 3, 1, 4, 9, 3, 9, 1, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 3, 9, 3, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 3, 9, 1, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 9, 9, 1, 2, 9, 9 },
			{ 4, 3, 1, 4, 9, 3, 9, 1, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 3, 9, 3, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 3, 9, 1, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 9, 9, 1, 2, 9, 9 },
			{ 1, 3, 2, 2, 2, 4, 9, 9, 9, 2, 4 },
			{ 3, 3, 3, 2, 4, 4, 9, 3, 9, 4, 4 },
			{ 4, 3, 1, 2, 3, 3, 9, 1, 9, 3, 3 },
			{ 4, 3, 1, 4, 9, 3, 9, 1, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 3, 9, 3, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 3, 9, 1, 9, 3, 3 },
			{ 4, 3, 3, 3, 9, 9, 9, 1, 2, 9, 9 },
			{ 4, 3, 3, 3, 3, 3, 3, 1, 2, 3, 9 },

		};

		int queueLenght = numbers.GetLength (0) * numbers.GetLength (1);
		int queueIndex = -1;
		char[,] visited = new char[numbers.GetLength (0), numbers.GetLength (1)];
		int[,] queue = new int[2, queueLenght];


		for (int i = 0; i < queue.GetLength (0); i++) {
			for (int y = 0; y < queue.GetLength (1); y++) {
				queue [i, y] = -1;
			}
		}

		int currentSize = 0, maxSize = 0, rowChecked = 0, colChecked = 0;
		for (int rowStarting = 0; rowStarting < numbers.GetLength (0); rowStarting++) {
			for (int colStarting = 0; colStarting < numbers.GetLength (1); colStarting++) {
				// if already visited = get next
				if (visited [rowStarting, colStarting] == 'y') {
					continue;                                        
				}

				//get next type of element i.e. utill now was '1' next is '3'
				rowChecked = rowStarting;
				colChecked = colStarting;

				do {
					//check if current is as the first
					if (numbers [rowStarting, colStarting] == numbers [rowChecked, colChecked] && visited [rowChecked, colChecked] != 'y') {
						currentSize++;

						//put in the same row and col of array "visited an 'y'"
						//so we don't check(use it as starting element) it again
						visited [rowChecked, colChecked] = 'y';

						//check upper and put in queue if equal and not visited already
						if (rowChecked - 1 >= 0) {
							if (numbers [rowChecked - 1, colChecked] == numbers [rowStarting, colStarting] && visited [rowChecked - 1, colChecked] != 'y') {
								queueIndex++;
								queue [0, queueIndex] = rowChecked - 1;
								queue [1, queueIndex] = colChecked;                                
							}
						}

						//check down - add to queue if not visited already
						if (rowChecked + 1 < numbers.GetLength (0)) {
							if (numbers [rowChecked + 1, colChecked] == numbers [rowStarting, colStarting] && visited [rowChecked + 1, colChecked] != 'y') {
								queueIndex++;
								queue [0, queueIndex] = rowChecked + 1;
								queue [1, queueIndex] = colChecked;                                
							}
						}

						//check left - add to queue if not visited already
						if (colChecked - 1 >= 0) {
							if (numbers [rowChecked, colChecked - 1] == numbers [rowStarting, colStarting] && visited [rowChecked, colChecked - 1] != 'y') {
								queueIndex++;
								queue [0, queueIndex] = rowChecked;
								queue [1, queueIndex] = colChecked - 1;                                
							}
						}

						//check right - add to queue if not visited already
						if (colChecked + 1 < numbers.GetLength (1)) {
							if (numbers [rowChecked, colChecked + 1] == numbers [rowStarting, colStarting] && visited [rowChecked, colChecked + 1] != 'y') {
								queueIndex++;
								queue [0, queueIndex] = rowChecked;
								queue [1, queueIndex] = colChecked + 1;

							}
						}                        

					}

					//if there are any elements in queue and they are
					//valid array index identifiers i.e. not -1 but 0,1,2,3....
					if (queueIndex > -1 && queue [0, queueIndex] != -1) {
						//take the row and col from queue and give -1
						//to the queue - delete the queue entry

						rowChecked = queue [0, queueIndex];
						queue [0, queueIndex] = -1;
						colChecked = queue [1, queueIndex];
						queue [1, queueIndex] = -1;  
						queueIndex--;
					} else {
						break;
					}
				} while (queueIndex > -2);


				if (currentSize > maxSize) {
					maxSize = currentSize;
				}
				currentSize = 0;
			}
		}

		print (maxSize);
	}
}

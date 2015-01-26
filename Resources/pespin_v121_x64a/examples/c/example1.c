//Bubble Sort in C
//An example on how to use PESpin License markers 


#include <stdio.h>
#include <iostream.h>


int License_code(unsigned __int64 a, unsigned __int64 b) 
{
  return 0;
}         


void bubbleSort(int *array,int length)//Bubble sort function 
{
  int i,j;
    
  License_code(0xC0DE064011111111, 0xC0DE064022222222);

    
	for(i=0;i<10;i++)
	{
		for(j=0;j<i;j++)
		{
			if(array[i]>array[j])
			{
				int temp=array[i];
				array[i]=array[j];
				array[j]=temp;
			}

		}

	}
    
  License_code(0xC0DE064044444444, 0xC0DE064088888888);

}

void printElements(int *array,int length)
{
    int i=0;
    for(i=0;i<10;i++)
      printf("%d ", array[i]);
}


void main()
{

  int a[]={9,6,5,23,2,6,2,7,1,8};
  
  bubbleSort(a,10);
  printElements(a,10);
  
}


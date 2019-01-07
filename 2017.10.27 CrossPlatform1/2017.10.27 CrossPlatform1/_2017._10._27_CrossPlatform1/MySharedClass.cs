using System;

namespace _2017._10._27_CrossPlatform1
{
    public class MySharedClass
    {
        public MySharedClass()
        {
        }

        public int Count { get; set; } = 0;
        // Increment Count by 1
        public string IncreaseCount()
        {
            Count++;
            return $"You clicked {Count} times";
        }
    }
}


namespace SortingAlgorithmAnimation.SortingAlgorithms;

/// <summary>
/// 排序算法静态类 - 算法都是网上抄的
/// <para>添加算法需要遵循以下规则才能正常记录运行数据:</para>
/// <para>读写值使用索引器 / 交换元素使用Swap方法 / 需要辅助数组或列表使用ExternalArray或ExternalList</para>
/// <para>添加方法需要遵循以下规则才能被自动添加到下拉框:</para>
/// <para>方法为static, 名称包含Sort / 入参为单个ListeningList&lt;int&gt;</para>
/// </summary>
public static class SortingAlgorithms
{
    /// <summary>
    /// 冒泡
    /// </summary>
    /// <param name="list"></param>
    public static void BubbleSort(ListeningList<int> list)
    {
        var n = list.Count;
        for (var i = 0; i < n - 1; i++)
        for (var j = 0; j < n - i - 1; j++)
            if (list[j] > list[j + 1])
                list.Swap(j, j + 1);
    }

    /// <summary>
    /// 选择
    /// </summary>
    /// <param name="list"></param>
    public static void SelectSort(ListeningList<int> list)
    {
        for (var i = 0; i < list.Count - 1; i++)
        {
            var min = list[i];
            var minIndex = i;

            for (var j = i + 1; j < list.Count; j++)
                if (min > list[j])
                {
                    min = list[j];
                    minIndex = j;
                }

            list.Swap(i, minIndex);
        }
    }

    /// <summary>
    /// 插入
    /// </summary>
    /// <param name="list"></param>
    public static void InsertionSort(ListeningList<int> list)
    {
        var arrayLength = list.Count;
        for (var i = 1; i < arrayLength; ++i)
        {
            var temp = list[i];
            var j = i - 1;

            while (j >= 0 && list[j] > temp)
            {
                list[j + 1] = list[j];
                j--;
            }

            list[j + 1] = temp;
        }
    }

    /// <summary>
    /// 快速排序
    /// </summary>
    /// <param name="list"></param>
    public static void QuickSort(ListeningList<int> list)
    {
        QuickSort(list, 0, list.Count - 1);
    }

    private static void QuickSort(ListeningList<int> list, int left, int right)
    {
        var i = left;
        var j = right;
        var pivot = list[left];
        while (i <= j)
        {
            while (list[i] < pivot) i++;

            while (list[j] > pivot) j--;
            if (i <= j)
            {
                list.Swap(i, j);
                i++;
                j--;
            }
        }

        if (left < j)
            QuickSort(list, left, j);
        if (i < right)
            QuickSort(list, i, right);
    }

    /// <summary>
    /// 堆排序
    /// </summary>
    /// <param name="list"></param>
    public static void HeapSort(ListeningList<int> list)
    {
        for (var i = list.Count / 2 - 1; i >= 0; i--)
            Heapify(list, list.Count, i);
        for (var i = list.Count - 1; i >= 0; i--)
        {
            (list[0], list[i]) = (list[i], list[0]);
            Heapify(list, i, 0);
        }
    }

    private static void Heapify(ListeningList<int> list, int n, int i)
    {
        var largest = i;
        var left = 2 * i + 1;
        var right = 2 * i + 2;
        if (left < n && list[left] > list[largest])
            largest = left;
        if (right < n && list[right] > list[largest])
            largest = right;
        if (largest != i)
        {
            (list[i], list[largest]) = (list[largest], list[i]);
            Heapify(list, n, largest);
        }
    }

    /// <summary>
    /// 希尔排序
    /// </summary>
    /// <param name="list"></param>
    public static void ShellSort(ListeningList<int> list)
    {
        var arrLength = list.Count;

        var gap = arrLength / 2;

        while (gap > 0)
        {
            for (var i = gap; i < arrLength; i++)
            {
                var temp = list[i];
                var j = i;

                while (j >= gap && list[j - gap] > temp)
                {
                    list[j] = list[j - gap];
                    j -= gap;
                }

                list[j] = temp;
            }

            gap /= 2;
        }
    }

    /// <summary>
    /// 归并排序
    /// </summary>
    /// <param name="list"></param>
    public static void MergeSort(ListeningList<int> list)
    {
        MergeSort(list, 0, list.Count - 1);
    }

    private static void MergeSort(ListeningList<int> list, int left, int right)
    {
        if (left < right)
        {
            var mid = (left + right) / 2;

            MergeSort(list, left, mid);

            MergeSort(list, mid + 1, right);

            Merge(list, left, mid, right);
        }
    }

    private static void Merge(ListeningList<int> list, int left, int mid, int right)
    {
        var n1 = mid - left + 1;
        var n2 = right - mid;

        var leftArr = list.ExternalArray(n1);
        var rightArr = list.ExternalArray(n2);

        for (var i = 0; i < n1; ++i) leftArr[i] = list[left + i];

        for (var j = 0; j < n2; ++j) rightArr[j] = list[mid + 1 + j];

        var k = left;
        var p = 0;
        var q = 0;

        while (p < n1 && q < n2)
        {
            if (leftArr[p] <= rightArr[q])
            {
                list[k] = leftArr[p];
                p++;
            }
            else
            {
                list[k] = rightArr[q];
                q++;
            }

            k++;
        }

        while (p < n1)
        {
            list[k] = leftArr[p];
            p++;
            k++;
        }

        while (q < n2)
        {
            list[k] = rightArr[q];
            q++;
            k++;
        }
    }

    /// <summary>
    /// 计数排序
    /// </summary>
    /// <param name="list"></param>
    public static void CountingSort(ListeningList<int> list)
    {
        var arrayLength = list.Count;
        if (arrayLength <= 1) return;

        var min = list[0];
        var max = list[0];

        for (var i = 1; i < arrayLength; i++)
        {
            if (list[i] < min) min = list[i];
            if (list[i] > max) max = list[i];
        }

        var count = list.ExternalArray(max - min + 1);

        for (var i = 0; i < arrayLength; i++) count[list[i] - min]++;

        for (var i = 1; i < count.Length; i++) count[i] += count[i - 1];

        var temp = list.ExternalArray(arrayLength);

        for (var i = arrayLength - 1; i >= 0; i--)
        {
            var index = count[list[i] - min] - 1;
            temp[index] = list[i];
            count[list[i] - min]--;
        }

        for (var i = 0; i < arrayLength; i++) list[i] = temp[i];
    }

    /// <summary>
    /// 桶排序
    /// </summary>
    /// <param name="list"></param>
    public static void BucketSort(ListeningList<int> list)
    {
        var arrLength = list.Count;
        if (arrLength <= 1) return;

        var maxValue = list[0];
        var minValue = list[0];

        for (var i = 1; i < arrLength; i++)
        {
            if (list[i] > maxValue)
                maxValue = list[i];
            if (list[i] < minValue)
                minValue = list[i];
        }

        var bucketCount = (maxValue - minValue) / arrLength + 1;

        var buckets = new List<ExternalList<int>>(bucketCount);
        for (var i = 0; i < bucketCount; i++) buckets.Add(list.ExternalList());

        for (var i = 0; i < arrLength; i++)
        {
            var bucketIndex = (list[i] - minValue) / arrLength;
            buckets[bucketIndex].Add(list[i]);
        }

        var index = 0;
        for (var i = 0; i < bucketCount; i++)
        {
            if (buckets[i].Count == 0) continue;

            var tempArr = new ListeningList<int>(buckets[i]);
            QuickSort(tempArr);

            foreach (var num in tempArr) list[index++] = num;
        }
    }

    /// <summary>
    /// 基数排序
    /// </summary>
    /// <param name="list"></param>
    public static void RadixSort(ListeningList<int> list)
    {
        if (list.Count < 2)
            return;

        var max = GetMaxValue(list);

        for (var exp = 1; max / exp > 0; exp *= 10) CountingSort(list, exp);
    }

    private static void CountingSort(ListeningList<int> list, int exp)
    {
        var arrayLength = list.Count;
        var output = list.ExternalArray(arrayLength);
        var count = list.ExternalArray(10);

        for (var i = 0; i < arrayLength; i++) count[list[i] / exp % 10]++;

        for (var i = 1; i < 10; i++) count[i] += count[i - 1];

        for (var i = arrayLength - 1; i >= 0; i--)
        {
            output[count[list[i] / exp % 10] - 1] = list[i];
            count[list[i] / exp % 10]--;
        }

        for (var i = 0; i < arrayLength; i++) list[i] = output[i];
    }

    private static int GetMaxValue(ListeningList<int> list)
    {
        var max = list[0];
        for (var i = 1; i < list.Count; i++)
            if (list[i] > max)
                max = list[i];
        return max;
    }
}
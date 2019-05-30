using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//A*寻路
public class AStar : MonoBehaviour
{

    private Map map = new Map();//格子地图

    ////开启列表    
    private List<MapPoint> open_List = new List<MapPoint>();

    //关闭列表
    private List<MapPoint> close_List = new List<MapPoint>();

    //定义一个路径数组
    private ArrayList way = new ArrayList();


    //判断某点是否在开启列表中
    private bool IsInOpenList(int x, int y)
    {
        foreach (var v in open_List)
        {
            if (v.x == x && v.y == y)
                return true;
        }
        return false;
    }

    //判断某点是否在关闭列表中
    private bool IsInCloseList(int x, int y)
    {
        foreach (var v in close_List)
        {
            if (v.x == x && v.y == y)
                return true;
        }
        return false;
    }


    //从开启列表中找到那个F值最小的格子
    private MapPoint FindMinFInOpenList()
    {
        MapPoint minPoint = null;

        foreach (var v in open_List)
        {
            if (minPoint == null || minPoint.GetF > v.GetF)
                minPoint = v;
        }
        return minPoint;
    }


    //从开启列表中找到格子
    private MapPoint FindInOpenList(int x, int y)
    {

        foreach (var v in open_List)
        {
            if (v.x == x && v.y == y)
                return v;
        }
        return null;

    }
    /// <summary>
    /// a星算法寻路
    /// </summary>
    /// <returns>寻到的路结果.</returns>
    /// <param name="starPoint">起点    </param>
    /// <param name="targetPoint">终点</param>
    /// 
    public Vector2[] AStarFindWay(Vector2 starPoint, Vector2 targetPoint)
    {
        //清空容器
        way.Clear();
        open_List.Clear();
        close_List.Clear();

        //初始化起点格子
        MapPoint starMapPoint = new MapPoint();
        starMapPoint.x = (int)starPoint.x;
        starMapPoint.y = (int)starPoint.y;

        //初始化终点格子
        MapPoint targetMapPoint = new MapPoint();
        targetMapPoint.x = (int)targetPoint.x;
        targetMapPoint.y = (int)targetPoint.y;

        //将起点格子添加到开启列表中
        open_List.Add(starMapPoint);

        //寻找最佳路径
        //当目标点不在打开路径中时或者打开列表为空时循环执行
        while (!IsInOpenList(targetMapPoint.x, targetMapPoint.y) || open_List.Count == 0)
        {
            //从开启列表中找到那个F值最小的格子
            MapPoint minPoint = FindMinFInOpenList();

            if (minPoint == null)
                return null;

            //将该点从开启列表中删除，同时添加到关闭列表中
            open_List.Remove(minPoint);
            close_List.Add(minPoint);

            //检查改点周边的格子
            CheckPerPointWithMap(minPoint, targetMapPoint);
        }

        //在开启列表中找到终点
        MapPoint endPoint = FindInOpenList(targetMapPoint.x, targetMapPoint.y);

        Vector2 everyWay = new Vector2(endPoint.x,endPoint.y);//保存单个路径点

        way.Add(everyWay);//添加到路径数组中

        //遍历终点，找到每一个父节点：即寻到的路
        while (endPoint.fatherPoint != null)
        {
            everyWay.x = endPoint.fatherPoint.x;
            everyWay.y = endPoint.fatherPoint.y;

            way.Add(everyWay);

            endPoint = endPoint.fatherPoint;
        }

        //将路径数组从倒序变成正序并返回
        Vector2[] ways = new Vector2[way.Count];
        for (int i = way.Count - 1; i >= 0; --i)
        {
            ways[way.Count - i - 1] = (Vector2)way[i];
        }

        //清空容器
        way.Clear();
        open_List.Clear();
        close_List.Clear();

        //返回正序的路径数组
        return ways;
    }

    //判断地图上某个坐标点是不是障碍点
    private bool IsBar(int x, int y)
    {
        
        //判断地图上某个坐标点是不是障碍点
        Vector2 p = new Vector2(x, y);

        //检测该点周边是否有障碍物
        Collider2D[] colliders = Physics2D.OverlapCircleAll(p, 0.5f);
        var f = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Item_Collision"){
                f = true;
                break;
            }
        }
        if (f)
            return true;//有障碍物，说明该点不可通过，是障碍物点
        return false;
    }

    //计算某方块的G值
    public int GetG(MapPoint p)
    {
        if (p.fatherPoint == null)
            return 0;

        if (p.x == p.fatherPoint.x || p.y == p.fatherPoint.y)
            return p.fatherPoint.G + 10;
        else
            return p.fatherPoint.G + 14;
    }

    //计算某方块的H值
    public int GetH(MapPoint p, MapPoint targetPoint)
    {
        return (Mathf.Abs(targetPoint.x - p.x) + Mathf.Abs(targetPoint.y - p.y)) * 10;
    }

    //检查某点周边的格子
    private void CheckPerPointWithMap(MapPoint _point, MapPoint targetPoint)
    {
        for (int i = _point.x - 1; i <= _point.x + 1; ++i)
        {
            for (int j = _point.y - 1; j <= _point.y + 1; ++j)
            {
                //剔除超过地图的点
                if (i < map.star_X || i > map.end_X || j < map.star_Y || j > map.end_Y)
                    continue;

                //剔除该点是障碍点：即周围有墙的点
                if (IsBar(i, j))
                    continue;

                //剔除已经存在关闭列表或者本身点
                if (IsInCloseList(i, j) || (i == _point.x && j == _point.y))
                    continue;

                //剩下的就是没有判断过的点了
                if (IsInOpenList(i, j))
                {
                    //如果该点在开启列表中
                    //找到该点
                    MapPoint point = FindInOpenList(i, j);

                    int G = 0;
                    //计算出该点新的移动代价
                    if (point.x == _point.x || point.y == _point.y)
                        G = point.G + 10;
                    else
                        G = point.G + 14;

                    //如果该点的新G值比前一次小
                    if (G < point.G)
                    {
                        //更新新的G点
                        point.G = G;
                        point.fatherPoint = _point;

                    }
                }
                else
                {
                    //如果该点不在开启列表内
                    //初始化该点，并将该点添加到开启列表中
                    MapPoint newPoint = new MapPoint();
                    newPoint.x = i;
                    newPoint.y = j;
                    newPoint.fatherPoint = _point;

                    //计算该点的G值和H值并赋值
                    newPoint.G = GetG(newPoint);
                    newPoint.H = GetH(newPoint, targetPoint);

                    //将初始化完毕的格子添加到开启列表中
                    open_List.Add(newPoint);

                }

            }
        }
    }

}

//地图类
public class Map
{
    public int star_X;// 横坐标起点
    public int star_Y;// 纵坐标起点
    public int end_X;// 横坐标终点
    public int end_Y;//纵坐标终点

    public Map()
    {
        star_X = -100;
        star_Y = -100;
        end_X = 100;
        end_Y = 100;
    }

}

//每一个格子的信息
public class MapPoint
{
    //F = G + H
    //G        从起点A移动到指定方格的移动代价，父格子到本格子代价：直线为10，斜线为14
    //H        使用 Manhattan 计算方法，    计算（当前方格到目标方格的横线上+竖线上所经过的方格数）* 10

    public int x;//格子的x坐标
    public int y;//格子的y坐标

    public int G;
    public int H;

    public int GetF
    {
        get
        {
            return G + H;
        }
    }

    public MapPoint fatherPoint;//父格子

    public MapPoint() { }

    public MapPoint(int _x, int _y, int _G, int _H, MapPoint _fatherPoint)
    {
        this.x = _x;
        this.y = _y;
        this.G = _G;
        this.H = _H;
        this.fatherPoint = _fatherPoint;
    }
}
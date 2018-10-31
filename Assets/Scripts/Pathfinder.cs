using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    [Serializable]
    public class Node 
    {

        public int gCost; //distance to startPos
        public int hCost; //distance to endPos
        public int fCost; //g+h
        public Vector2 coords;
        public Vector2 parent;

        public Node(int g, int h, Vector2 coord, Vector2 parentNode)
        {
            gCost = g;
            hCost = h;
            fCost = g+h;
            coords = coord;
            parent = parentNode;
        }
    }

    LinkedList<Node> open = new LinkedList<Node>();
    LinkedList<Node> closed = new LinkedList<Node>();

    public LayerMask blockingLayer;

    public Vector2[] GetPathList(Vector2 startPos, Vector2 endPos)
    {
        Vector2[] pathList;

        if (!IsTraversable(endPos)||startPos == endPos)
        {
            pathList = new Vector2[1];
            pathList[0] = startPos;
            return pathList;
        }

        LinkedListNode<Node> theNode = FindPath(startPos, endPos);


        pathList = new Vector2[NumberOfParents(theNode)];

        for (int i = 0; i < pathList.Length; i++)
        {
            pathList[i] = theNode.Value.coords;
            theNode = GetNode(closed, theNode.Value.parent);
        }

        open.Clear();
        closed.Clear();

        return pathList;
    }


    LinkedListNode<Node> FindPath(Vector2 startPos, Vector2 endPos)
    {
        open.Clear();
        closed.Clear();

        //add start node to open
        Node theNode = new Node(0,(int) Vector2.Distance(endPos, startPos)*10, startPos, new Vector2(999,999));
        LinkedListNode<Node> theLinkedNode = new LinkedListNode<Node>(theNode);

        open.AddFirst(theLinkedNode);

        while (true)
        {
            //find node in open with lowest fcost
            LinkedListNode<Node> current = LowestFCost(open);
            
            open.Remove(current);
            closed.AddFirst(current);

            if(current.Value.coords == endPos) //path has been found
            {
                return current;
            }

            
            for (int i = 0; i < 8; i++)
            {


                Vector2 neighbour = new Vector2();
                int gCostAdd = 10;

                switch (i)
                {
                    case 0:
                        neighbour.x = current.Value.coords.x;
                        neighbour.y = current.Value.coords.y + 1;
                        break;
                    case 1:
                        neighbour.x = current.Value.coords.x + 1;
                        neighbour.y = current.Value.coords.y + 1;
                        gCostAdd = 14;
                        break;
                    case 2:
                        neighbour.x = current.Value.coords.x + 1;
                        neighbour.y = current.Value.coords.y;
                        break;
                    case 3:
                        neighbour.x = current.Value.coords.x + 1;
                        neighbour.y = current.Value.coords.y - 1;
                        gCostAdd = 14;
                        break;
                    case 4:
                        neighbour.x = current.Value.coords.x;
                        neighbour.y = current.Value.coords.y - 1;
                        break;
                    case 5:
                        neighbour.x = current.Value.coords.x - 1;
                        neighbour.y = current.Value.coords.y - 1;
                        gCostAdd = 14;
                        break;
                    case 6:
                        neighbour.x = current.Value.coords.x - 1;
                        neighbour.y = current.Value.coords.y;
                        break;
                    case 7:
                        neighbour.x = current.Value.coords.x - 1;
                        neighbour.y = current.Value.coords.y + 1;
                        gCostAdd = 14;
                        break;
                }

                //if neighbour is not traversable or neighbour is in Closed 
                //skip to next neighbour
                if (!FindInList(closed, neighbour) && IsTraversable(neighbour))
                {
                    //if new path to neighbour is shorter OR neighbour is not in open 
                    if (!FindInList(open, neighbour) || NumberOfParents(GetNode(open,neighbour)) > NumberOfParents(current) + 1)
                    {
                        
                        if (!FindInList(open, neighbour))
                        {
                            //Add node to open
                            Node neighbourNode = new Node(current.Value.gCost + gCostAdd, (int)Vector2.Distance(endPos, startPos)* 10, neighbour, current.Value.coords);
                            LinkedListNode<Node> neighbourLinkedNode = new LinkedListNode<Node>(neighbourNode);
                            open.AddFirst(neighbourLinkedNode);
                            
                        }
                        else
                        {
                            LinkedListNode<Node> neighbourLinkedNode = GetNode(open, neighbour);
                            open.Remove(neighbourLinkedNode);
                            Node neighbourNode = new Node(current.Value.gCost + gCostAdd, (int)Vector2.Distance(endPos, startPos) *10, neighbour, current.Value.coords);
                            neighbourLinkedNode = new LinkedListNode<Node>(neighbourNode);
                            open.AddFirst(neighbourNode);
                        }
                    }
                }
            }
        }
    }

    private bool IsTraversable(Vector2 coords)
    {
        RaycastHit2D hit = Physics2D.Linecast(coords, coords, blockingLayer);
        
        if (hit.transform == null)
            return true;
        else
            return false;
    }

    LinkedListNode<Node> GetNode(LinkedList<Node> ll, Vector2 toFind)
    {
        LinkedListNode<Node> current = ll.First;

        while (current != null)
        {
            if (current.Value.coords == toFind)
                return current;
            current = current.Next;
        }
        return null;

    }


    bool FindInList(LinkedList<Node> ll, Vector2 toFind)
    {

        LinkedListNode<Node> current = ll.First;

        for (int i = 0; i < ll.Count; i++)
        {
            if (current.Value.coords == toFind)
                return true;
            current = current.Next;
        }
        return false;
    }

    LinkedListNode<Node> LowestFCost(LinkedList<Node> ll)
    {

        if(ll.Count <= 1)
        {
            return ll.First;
        }
        
        LinkedListNode<Node> lowest = ll.First;
        LinkedListNode<Node> current = lowest.Next;

        while (current != null)
        {

            if (current.Value.fCost < lowest.Value.fCost || current.Value.fCost == lowest.Value.fCost && current.Value.hCost < lowest.Value.hCost)
            {
                lowest = current;
            }
            current = current.Next;
        }

        return lowest;
    }


    int NumberOfParents(LinkedListNode<Node> theNode)
    {
        int count = 0;

        while (theNode.Value.parent != new Vector2(999, 999))
        {
            theNode = GetNode(closed, theNode.Value.parent);
            count++;
        }
        return count;
    }

}

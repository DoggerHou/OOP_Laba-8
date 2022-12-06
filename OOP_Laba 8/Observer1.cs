﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace OOP_Laba_8
{
    public class Observerable
    {
        private List<IObserver> _observers;

        public Observerable()
        {
            _observers = new List<IObserver>();
        }

        public void addObserver(IObserver o)
        {
            _observers.Add(o);
        }


        public void notifyEveryone(Storage o)
        {
            foreach (var obj in _observers)
                obj.OnStorageChanged(this, o);
        }


        public void notifyObjects(Model who, int _x, int _y)
        {
            foreach (var obj in _observers)
                obj.OnObjectChanged(who, _x, _y);
        }

        public TreeNode gett()
        {
            return ((TreeObserver)_observers[0]).getNode();
        }
    }


    public interface IObserver
    {
        void OnStorageChanged(Observerable who, Storage o);
        void OnObjectChanged(Model who, int _x, int _y);
    }



    public class TreeObserver : IObserver
    {
        public TreeNode tn;


        public TreeObserver()
        {
            tn = new TreeNode();
        }


        public void processNode(TreeNode tn, Model m)
        {
            if (m is Group)
            {
                TreeNode child = new TreeNode();
                child.Text = m.GetType().ToString();
                tn.Nodes.Add(child);
                tn.LastNode.Checked = m.isDetailed();
                List<Model> aaa = ((Group)m).getGroup();
                foreach (var obj in aaa)
                    processNode(child, obj);
            }
            else
            {
                tn.Nodes.Add(m.GetType().ToString());
                tn.LastNode.Checked = m.isDetailed();
            }

        }

        public void OnStorageChanged(Observerable who, Storage o)
        {
            tn.Nodes.Clear();
            tn.Text = "Storage";
            for(int i = 0;i<o.getStorageSize();i++)
                processNode(tn, o.getObject(i));
        }


        public TreeNode getNode()
        {
            return tn;
        }


        public void OnObjectChanged(Model who, int _x, int _y) {}
    }


    public class StickyObserver : IObserver
    {
        Storage stickyStorage;
        public StickyObserver(Storage o)
        {
            stickyStorage = o;
        }
        public void OnObjectChanged(Model who, int _x, int _y)
        {
            for(int i =0;i<stickyStorage.getStorageSize(); i++)
            {
                if(stickyStorage.getObject(i) != who)
                    if(stickyStorage.getObject(i).isSticked(who))
                        stickyStorage.getObject(i).MoveObject(_x, _y);
            }
        }

        public void OnStorageChanged(Observerable who, Storage o) { }

    }
}

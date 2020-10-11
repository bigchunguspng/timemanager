﻿using System;
using System.Collections.Generic;

namespace TimeManager.Model.Data
{
    public class List
    {
        public string Name { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();

        public void TestTasks() //todo normal creation...
        {
            Tasks.Add(new Task(){Description = "make some stuff83578"});
            Tasks.Add(new Task(){Description = "make some stuft4t4"});
            Tasks.Add(new Task(new DateTime(2020, 11, 11)){Description = "make some stuff to 11.11"});
        }
    }
}
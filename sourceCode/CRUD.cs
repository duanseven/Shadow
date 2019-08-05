using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple
{
    public class CRUD
    {
        public void Create()
        {
            ClassEntity newclass = new ClassEntity { Name = "东方网力" };
            var db = DBFactory.CreateDBQuery<ClassEntity>();
            db.Save(newclass);
        }

        public void Modify(int id)
        {
            var db = DBFactory.CreateDBQuery<ClassEntity>();
            var entity = db.Get(id);
            entity.Name = "东方网力科技股份有限公司";
            db.Save(entity);
        }

        public void Delete()
        {
            var db = DBFactory.CreateDBQuery<ClassEntity>();
            db.DeleteKey(1);
        }

        public void Update()
        {
            var db = DBFactory.CreateDBQuery<ClassEntity>();
            var up = db.CreateUpdate();
            up.AddColumn(ClassTable._name, 1).Where(ClassTable._id == 1);
            db.Update(up);
        }

        public void Query()
        {
            var db = DBFactory.CreateDBQuery<ClassEntity>();
            var query = db.CreateQuery();
            query.Where(ClassTable._id > 1);
            var list = db.ToList(query);
            foreach (var classEntity in list)
            {
                Console.WriteLine(classEntity.Name);
            }
        }

        public void Query2()
        {
            var db = DBFactory.CreateDBQuery<ClassEntity>();
            var query = db.CreateQuery();
            query.Where(ClassTable._id > 1&&ClassTable._name.StartsWith("ff"));
            var list = db.ToList(query);
            foreach (var classEntity in list)
            {
                Console.WriteLine(classEntity.Name);
            }
        }

    }
}

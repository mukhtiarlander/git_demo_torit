namespace RDN.Raspberry.Models.OutModel
{
    public class GenericSingleModel<T> : Base
    {
        public GenericSingleModel()
        {
            
        }

        public GenericSingleModel(T model)
        {
            Model = model;
        }

        public T Model { get; set; }
    }
}
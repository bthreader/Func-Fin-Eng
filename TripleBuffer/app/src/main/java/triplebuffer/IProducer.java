package triplebuffer;

public interface IProducer<T> {
    public void write(T value);
}

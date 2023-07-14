package triplebuffer.buffer;

public interface IProducer<T> {
    public void write(T value);
}

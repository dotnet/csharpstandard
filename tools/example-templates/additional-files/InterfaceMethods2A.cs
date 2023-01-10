interface I<T> {}
class B {}
class D : B {}
class E : B {}
class C : I<D>
{
    public void M<U>() {} 
}

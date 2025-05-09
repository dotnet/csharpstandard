class RightShift
{
    void RightShift()
    {
      // valid
      a = b >> c;
      d >>= e;
      
      // invalid
      a = b > > c;
      d > >= e;      
    }
}
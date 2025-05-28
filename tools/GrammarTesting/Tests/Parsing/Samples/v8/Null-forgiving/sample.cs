class NullForgivingChecks
{
   void NullForgivingChecks()
   {
      // simple null-forgiving
      string? s = null;
      string a = s!;
      
      // null-conditional with null-forgiving
      int b = c?.d.e;
      int b = c?.d!.e;
      int b = c?.d.e.f;
      int b = c?.d.e!.f;
      
      // null-forgiving semantic errors – cannot apply to same expression twice
      // all of these parse but should raise a semantic warning
      string b = s!!;
      string c = (s!)!;
      string d = (s)!!;
      string e = ((s)!)!;
      
      // check the semantic check isn’t fooled by a `!)!` sequence, this is valid
      string f = (s ?? s!)!;
   }
}
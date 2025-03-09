#if false
using MarkStudio.QQ;

public class 入群验证
{
    public void 验证(ref int 机会, int QQ等级, int 违规记录, bool 安卓玩家, bool 只要包不想交流, string 理由)
    {
        if (机会 == 0 || QQ等级 <= 5 || 违规记录 != 0 || (安卓玩家 && 只要包不想交流))
        {
            永久拒绝();
            return;
        }

        机会 = 理由 switch
        {
            "答案错误" or "漏答错答" => 1,
            "黑名单" => 0,
            _ => 机会
        };

        if (机会 == 0)
            永久拒绝();
        else
            拒绝();
    }

    private void 拒绝() { }

    private void 永久拒绝() { }
}
#endif
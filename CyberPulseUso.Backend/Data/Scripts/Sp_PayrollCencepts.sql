
CREATE OR ALTER PROCEDURE Lnpa.GetPayrollConceptsByCharge
    @Id INT
    WITH ENCRYPTION
AS
BEGIN
    SET NOCOUNT ON;

    -- Conceptos asociados a los parámetros de remuneración del cargo.
    SELECT CAST('' AS VARCHAR(450)) AS UserId, @Id AS ChargeId, A.Id AS PayrollConceptId, A.Name, 0 AS Amount, V.Value AS UnitValue,0 AS AccruedValue,0 AS DeductedValue,A.StatuId
    FROM Lnpa.PayrollConcepts AS A
        INNER JOIN Lnpa.LaborRemunerationParameters AS P ON P.Id = A.LRPId
        INNER JOIN Lnpa.LaborRemunerationParametersValues AS V ON V.LRPId = P.Id
        INNER JOIN Lnpa.Groups AS G ON G.Id = V.GroupId
        INNER JOIN Lnpa.Charges AS C ON C.GroupId = G.Id
    WHERE
        C.Id = @Id
        AND V.LRPId IN (4, 5, 8, 16, 17, 18, 19, 20)

    UNION ALL

    -- Conceptos que no pertenecen a la lista excluida.

    SELECT
        CAST('' AS VARCHAR(450)) AS UserId,
        @Id AS ChargeId,
        A.Id AS PayrollConceptId,
        A.Name,
        0 AS Amount,
        0 AS UnitValue,
        0 AS AccruedValue,
        0 AS DeductedValue,
        A.StatuId
    FROM Lnpa.PayrollConcepts AS A
    WHERE
        A.Id NOT IN (1, 2, 3, 4, 7, 8, 9, 10);
END;